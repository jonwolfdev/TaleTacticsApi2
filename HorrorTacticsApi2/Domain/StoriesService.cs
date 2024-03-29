﻿using HorrorTacticsApi2.Data;
using HorrorTacticsApi2.Data.Entities;
using HorrorTacticsApi2.Domain.Dtos;
using HorrorTacticsApi2.Domain.Exceptions;
using HorrorTacticsApi2.Domain.Models.Audio;
using HorrorTacticsApi2.Domain.Models.Stories;
using Microsoft.EntityFrameworkCore;

namespace HorrorTacticsApi2.Domain
{
    public class StoriesService
    {
        readonly IHorrorDbContext _context;
        readonly StoryModelEntityHandler _imeHandler;
        readonly UserService _service;
        public StoriesService(IHorrorDbContext context, StoryModelEntityHandler handler, UserService service)
        {
            _service = service;
            _context = context;
            _imeHandler = handler;
        }

        public async Task<IList<ReadStoryModel>> GetAllStoriesAsync(UserJwt user, CancellationToken token)
        {
            var list = new List<ReadStoryModel>();
            var images = await GetQuery(user.Id, true).ToListAsync(token);
            images.ForEach(image => { list.Add(_imeHandler.CreateReadModel(image)); });

            return list;
        }

        public async Task<ReadStoryModel?> TryGetAsync(UserJwt user, long id, CancellationToken token)
        {
            var entity = await TryFindStoryAsync(user.Id, id, true, token);
            return entity == default ? default : _imeHandler.CreateReadModel(entity);
        }

        public async Task<ReadStoryModel> CreateStoryAsync(UserJwt user, CreateStoryModel model, bool basicValidated, CancellationToken token)
        {
            _imeHandler.Validate(model, basicValidated);

            var entity = _imeHandler.CreateEntity(_service.GetReference(user), model);
            _context.Stories.Add(entity);
            await _context.SaveChangesWrappedAsync(token);

            return _imeHandler.CreateReadModel(entity);
        }

        public async Task<ReadStoryModel> UpdateStoryAsync(UserJwt user, long id, UpdateStoryModel model, bool basicValidated, CancellationToken token)
        {
            _imeHandler.Validate(model, basicValidated);
            // TODO: improve includeAll performance (does it really need to include all references when Put?)
            // - Even better yet... why return data when updating?
            var entity = await TryFindStoryAsync(user.Id, id, true, token);
            if (entity == default)
                throw new HtNotFoundException($"Story with Id {id} not found");

            _imeHandler.UpdateEntity(model, entity);

            await _context.SaveChangesWrappedAsync(token);

            return _imeHandler.CreateReadModel(entity);
        }

        public async Task DeleteStoryAsync(UserJwt user, long id, CancellationToken token)
        {
            var entity = await TryFindStoryAsync(user.Id, id, false, token);
            if (entity == default)
                throw new HtNotFoundException($"Story with Id {id} not found");

            _context.Stories.Remove(entity);
            await _context.SaveChangesWrappedAsync(token);
        }

        public async Task<StoryEntity?> TryFindStoryAsync(long? userId, long id, bool includeAll, CancellationToken token)
        {
            var entity = await GetQuery(userId, includeAll).SingleOrDefaultAsync(x => x.Id == id, token);

            return entity;
        }

        IQueryable<StoryEntity> GetQuery(long? userId, bool includeAll = true)
        {
            IQueryable<StoryEntity> query = _context.Stories;
            if (userId.HasValue)
                query = query.Where(x => x.Owner.Id == userId);

            if (includeAll)
            {
                // TODO: this should be organized (code)
                query = query
                        .Include(x => x.Scenes)
                            .ThenInclude(x=>x.Commands)
                                .ThenInclude(x => x.Audios)
                                .ThenInclude(x => x.File)
                        .Include(x => x.Scenes)
                            .ThenInclude(x => x.Commands)
                                .ThenInclude(x => x.Images)
                                .ThenInclude(x => x.File)
                    ;
            }
            return query;
        }
    }
}
