﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HorrorTacticsApi2.Tests3.Api.Helpers;
using Xunit;
using HorrorTacticsApi2.Domain.Models.Stories;
using HorrorTacticsApi2.Domain.Models.Games;

namespace HorrorTacticsApi2.Tests3.Api
{
    public class StoriesControllerCRUDTests : IClassFixture<ApiTestsCollection>
    {
        readonly ApiTestsCollection _collection;
        const string Path = "secured/stories";
        const string PathStoryScenes = "secured/storyscenes";
        const string PathGames = "games";

        public StoriesControllerCRUDTests(ApiTestsCollection apiTestsCollection)
        {
            _collection = apiTestsCollection;
        }

        [Fact]
        public async Task Should_Do_Crud_Without_Errors()
        {
            using var client = _collection.CreateClient();

            var storyDto = await Post_Should_Create_Story(client, "story title", "Description ñ");
            var updatedDto = await Put_Should_Update_Story(client, "story changed ñ", "Description changed", storyDto);

            var imageDto = await ImagesControllerCRUDTests.Post_Should_Create_Image(client, "imagex");
            var imageDto2 = await ImagesControllerCRUDTests.Post_Should_Create_Image(client, "imagex2");

            var audioDto = await AudioControllerCRUDTests.Post_Should_Create_Audio(client, "audio");
            //var audioDto2 = await AudioControllerCRUDTests.Post_Should_Create_Audio(client, "audio2");

            var createStoryScene = new CreateStorySceneModel(
                new List<string>(){ "Text ñ", "Hola ñ" },
                default,
                default,
                new List<long>() { imageDto.Id },
                new List<long>() { audioDto.Id }
            );

            var storySceneModel = await Post_Should_Create_StoryScene(client, createStoryScene, storyDto.Id);
            var storySceneModel2 = await Post_Should_Create_StoryScene(client, createStoryScene, storyDto.Id);

            // TODO: update storyscene2 with the same images
            var updateStoryScene = new UpdateStorySceneModel(
                new List<string>() { "Text ñ", "Hola ñ", "ñ ñ" },
                default,
                default,
                new List<long>() { imageDto.Id, imageDto2.Id },
                default
            );

            var createdGame = await Post_Should_Create_Game(client, storyDto.Id);
            // TODO: move this to its own test collection
            await Games_Get_Should_Return_GameConfiguration(client, createdGame.GameCode);

            await Delete_Should_Delete_StoryScene(client, storyDto.Id, storySceneModel2.Id);
            await Get_Should_Return_StoryScene_NotFound(client, storyDto.Id, storySceneModel2.Id);

            var updatedSceneModel = await Put_Should_Update_StoryScene(client, updateStoryScene, storyDto.Id, storySceneModel.Id);

            var readSceneModel = await Get_Should_Return_StoryScene(client, storyDto.Id, storySceneModel.Id, updatedSceneModel);
            var getReadStoryModel = await Get_Should_Return_Story(client, storyDto.Id, updatedDto, readSceneModel);
            var listStories = await Get_Should_Return_Stories(client, updatedDto, readSceneModel);

            // TODO: move this to its own test collection
            await StoryScenes_Get_Should_Return_StoryScene(client, readSceneModel.Id, readSceneModel);

            await Delete_Should_Delete_Story(client, storyDto.Id);
            await Get_Should_Return_Story_NotFound(client, storyDto.Id);
            await Get_Should_Return_StoryScene_NotFound(client, storyDto.Id, storySceneModel.Id);

            await ImagesControllerCRUDTests.GetImageByIdAndAssertAsync(client, imageDto);
            await ImagesControllerCRUDTests.GetImageByIdAndAssertAsync(client, imageDto2);
            await AudioControllerCRUDTests.GetAudioByIdAndAssertAsync(client, audioDto);

            // TODO: check when deleting Image, Scene is not deleted
            // TODO: actually check the values (nested values)
        }

        static async Task<ReadGameCreatedModel> Post_Should_Create_Game(HttpClient client, long storyId)
        {
            // arrange
            var model = new CreateGameModel(storyId);

            // act
            using var response = await client.PostAsync(Path + "/game", Helper.GetContent(model));

            // assert
            var readModel = await Helper.VerifyAndGetAsync<ReadGameCreatedModel>(response, StatusCodes.Status201Created);
            Assert.True(readModel.GameCode.Length > 0);
            return readModel;
        }
        static async Task<ReadGameConfiguration> Games_Get_Should_Return_GameConfiguration(HttpClient client, string gameCode)
        {
            // arrange

            // act
            using var response = await client.GetAsync(PathGames + $"/{gameCode}/configuration");

            // assert
            var readModel = await Helper.VerifyAndGetAsync<ReadGameConfiguration>(response, StatusCodes.Status200OK);
            Assert.Equal(1, readModel.Audios.Count);
            Assert.Equal(1, readModel.Images.Count);
            return readModel;
        }
        static async Task<ReadStorySceneModel> StoryScenes_Get_Should_Return_StoryScene(HttpClient client, long storySceneId, ReadStorySceneModel model)
        {
            // arrange

            // act
            using var response = await client.GetAsync(PathStoryScenes + $"/{storySceneId}");

            // assert
            var readModel = await Helper.VerifyAndGetAsync<ReadStorySceneModel>(response, StatusCodes.Status200OK);
            Assert.Equal(model.Id, readModel.Id);
            Assert.Equal(model.Images?.Count, readModel.Images.Count);
            Assert.Equal(model.Audios?.Count, readModel.Audios.Count);
            Assert.Equal(model.Texts?.Count, readModel.Texts.Count);
            //Assert.Equal(model.Minigames?.Count, readModel.Minigames);
            Assert.Equal(model.Timers?.Count, readModel.Timers.Count);
            return readModel;
        }

        static async Task<ReadStorySceneModel> Get_Should_Return_StoryScene(HttpClient client, long storyId, long storySceneId, ReadStorySceneModel model)
        {
            // arrange
            
            // act
            using var response = await client.GetAsync(Path + $"/{storyId}/scenes/{storySceneId}");

            // assert
            var readModel = await Helper.VerifyAndGetAsync<ReadStorySceneModel>(response, StatusCodes.Status200OK);
            Assert.Equal(model.Id, readModel.Id);
            Assert.Equal(model.Images?.Count, readModel.Images.Count);
            Assert.Equal(model.Audios?.Count, readModel.Audios.Count);
            Assert.Equal(model.Texts?.Count, readModel.Texts.Count);
            //Assert.Equal(model.Minigames?.Count, readModel.Minigames);
            Assert.Equal(model.Timers?.Count, readModel.Timers.Count);
            return readModel;
        }

        static async Task Get_Should_Return_Story_NotFound(HttpClient client, long storyId)
        {
            // arrange

            // act
            using var response = await client.GetAsync(Path + $"/{storyId}");

            // assert
            Assert.Equal(StatusCodes.Status404NotFound, (int)response.StatusCode);
        }

        static async Task Get_Should_Return_StoryScene_NotFound(HttpClient client, long storyId, long storySceneId)
        {
            // arrange

            // act
            using var response = await client.GetAsync(Path + $"/{storyId}/scenes/{storySceneId}");

            // assert
            Assert.Equal(StatusCodes.Status404NotFound, (int)response.StatusCode);
        }

        static async Task<ReadStoryModel> Get_Should_Return_Story(HttpClient client, long storyId, ReadStoryModel modelStory, ReadStorySceneModel model)
        {
            // arrange

            // act
            using var response = await client.GetAsync(Path + $"/{storyId}");

            // assert
            var readModel = await Helper.VerifyAndGetAsync<ReadStoryModel>(response, StatusCodes.Status200OK);
            Assert.Equal(modelStory.Id, readModel.Id);
            Assert.Equal(modelStory.Description, readModel.Description);
            Assert.Equal(modelStory.Title, readModel.Title);

            Assert.Equal(1, readModel.StoryScenes.Count);
            Assert.Equal(model.Images?.Count, readModel.StoryScenes[0].Images.Count);
            Assert.Equal(model.Audios?.Count, readModel.StoryScenes[0].Audios.Count);
            Assert.Equal(model.Texts?.Count, readModel.StoryScenes[0].Texts.Count);
            //Assert.Equal(model.Minigames?.Count, readModel.Minigames);
            Assert.Equal(model.Timers?.Count, readModel.StoryScenes[0].Timers.Count);
            return readModel;
        }

        static async Task<List<ReadStoryModel>> Get_Should_Return_Stories(HttpClient client, ReadStoryModel modelStory, ReadStorySceneModel model)
        {
            // arrange

            // act
            using var response = await client.GetAsync(Path);

            // assert
            var list = await Helper.VerifyAndGetAsync<List<ReadStoryModel>>(response, StatusCodes.Status200OK);
            Assert.Single(list);
            var readModel = list[0];
            Assert.Equal(modelStory.Id, readModel.Id);
            Assert.Equal(modelStory.Description, readModel.Description);
            Assert.Equal(modelStory.Title, readModel.Title);

            Assert.Equal(1, readModel.StoryScenes.Count);
            Assert.Equal(model.Images?.Count, readModel.StoryScenes[0].Images.Count);
            Assert.Equal(model.Audios?.Count, readModel.StoryScenes[0].Audios.Count);
            Assert.Equal(model.Texts?.Count, readModel.StoryScenes[0].Texts.Count);
            //Assert.Equal(model.Minigames?.Count, readModel.Minigames);
            Assert.Equal(model.Timers?.Count, readModel.StoryScenes[0].Timers.Count);
            return list;
        }

        static async Task<ReadStoryModel> Post_Should_Create_Story(HttpClient client, string title, string desc)
        {
            // arrange
            var model = new CreateStoryModel(title, desc);

            // act
            using var response = await client.PostAsync(Path, Helper.GetContent(model));

            // assert
            var readModel = await Helper.VerifyAndGetAsync<ReadStoryModel>(response, StatusCodes.Status201Created);
            Assert.True(readModel.Id > 0);
            Assert.Equal(desc, readModel.Description);
            Assert.Equal(title, readModel.Title);
            Assert.Empty(readModel.StoryScenes);
            return readModel;
        }

        static async Task<ReadStorySceneModel> Post_Should_Create_StoryScene(HttpClient client, CreateStorySceneModel model, long storyId)
        {
            // arrange
            
            // act
            using var response = await client.PostAsync(Path + $"/{storyId}/scenes", Helper.GetContent(model));

            // assert
            var readModel = await Helper.VerifyAndGetAsync<ReadStorySceneModel>(response, StatusCodes.Status201Created);
            Assert.True(readModel.Id > 0);
            Assert.Equal(model.Images?.Count, readModel.Images.Count);
            Assert.Equal(model.Audios?.Count, readModel.Audios.Count);
            Assert.Equal(model.Texts?.Count, readModel.Texts.Count);
            //Assert.Equal(model.Minigames?.Count, readModel.Minigames);
            Assert.Equal(model.Timers?.Count ?? 0, readModel.Timers.Count);

            return readModel;
        }

        static async Task<ReadStoryModel> Put_Should_Update_Story(HttpClient client, string title, string desc, ReadStoryModel original)
        {
            // arrange
            var model = new UpdateStoryModel(title, desc);

            // act
            using var response = await client.PutAsync(Path + $"/{original.Id}", Helper.GetContent(model));

            // assert
            var readModel = await Helper.VerifyAndGetAsync<ReadStoryModel>(response, StatusCodes.Status200OK);
            Assert.True(readModel.Id > 0);
            Assert.Equal(desc, readModel.Description);
            Assert.Equal(title, readModel.Title);
            Assert.Equal(original.StoryScenes, readModel.StoryScenes);
            return readModel;
        }

        static async Task<ReadStorySceneModel> Put_Should_Update_StoryScene(HttpClient client, UpdateStorySceneModel model, long storyId, long storySceneId)
        {
            // arrange

            // act
            using var response = await client.PutAsync(Path + $"/{storyId}/scenes/{storySceneId}", Helper.GetContent(model));

            // assert
            var readModel = await Helper.VerifyAndGetAsync<ReadStorySceneModel>(response, StatusCodes.Status200OK);
            Assert.True(readModel.Id > 0);
            Assert.Equal(model.Images?.Count ?? 0, readModel.Images.Count);
            Assert.Equal(model.Audios?.Count ?? 1, readModel.Audios.Count);
            Assert.Equal(model.Texts?.Count ?? 0, readModel.Texts.Count);
            //Assert.Equal(model.Minigames?.Count, readModel.Minigames);
            Assert.Equal(model.Timers?.Count ?? 0, readModel.Timers.Count);
            return readModel;
        }

        static async Task Delete_Should_Delete_StoryScene(HttpClient client, long storyId, long storySceneId)
        {
            // arrange
            
            // act
            using var response = await client.DeleteAsync(Path + $"/{storyId}/scenes/{storySceneId}");

            // assert
            Assert.Equal(StatusCodes.Status204NoContent, (int)response.StatusCode);
        }

        static async Task Delete_Should_Delete_Story(HttpClient client, long storyId)
        {
            // arrange

            // act
            using var response = await client.DeleteAsync(Path + $"/{storyId}");

            // assert
            Assert.Equal(StatusCodes.Status204NoContent, (int)response.StatusCode);
        }
    }
}
