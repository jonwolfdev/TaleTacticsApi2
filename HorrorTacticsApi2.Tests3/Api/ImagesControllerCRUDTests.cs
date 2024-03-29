﻿using HorrorTacticsApi2.Domain.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HorrorTacticsApi2.Tests3.Api.Helpers;
using Xunit;
using HorrorTacticsApi2.Data.Entities;

namespace HorrorTacticsApi2.Tests3.Api
{
    public class ImagesControllerCRUDTests : IClassFixture<ApiTestsCollection>
    {
        public static readonly byte[] JpgImageBytes = Convert.FromBase64String("/9j/4AAQSkZJRgABAQEAYABgAAD/4QBGRXhpZgAATU0AKgAAAAgABAESAAMAAAABAAEAAFEAAAQAAAABAAAAAFEBAAMAAAABAAEAAFEEAAEAAAAB/AAAAAAAAAD/2wBDAAIBAQIBAQICAgICAgICAwUDAwMDAwYEBAMFBwYHBwcGBwcICQsJCAgKCAcHCg0KCgsMDAwMBwkODw0MDgsMDAz/2wBDAQICAgMDAwYDAwYMCAcIDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAwMDAz/wAARCAABAAEDASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD90KKKK/j8+gP/2Q==");
        readonly ApiTestsCollection _collection;
        const string Path = "secured/images";
        public ImagesControllerCRUDTests(ApiTestsCollection apiTestsCollection)
        {
            _collection = apiTestsCollection;
            _collection.WebAppFactory.Options.DbName = nameof(ImagesControllerCRUDTests);
        }

        [Fact]
        public async Task Should_Do_Crud_Without_Errors()
        {
            using var client = _collection.CreateClient();

            var readImageDto = await Post_Should_Create_Image(client, "image1");
            await Get_Should_Return_One_Image(client, readImageDto);
            var readImageDto2 = await Post_Should_Create_Image(client, "image2");
            var readImageDto1_1 = await Put_Should_Update_Image(client, readImageDto);

            await GetImageByIdAndAssertAsync(client, readImageDto2);
            await GetImageByIdAndAssertAsync(client, readImageDto1_1);

            await Get_Should_Return_Two_Images(client, readImageDto1_1, readImageDto2);

            await Delete_Should_Delete_Image(client, readImageDto1_1);

            await Get_Should_Return_One_Image(client, readImageDto2);

            await Delete_Should_Delete_Image(client, readImageDto2);
            // TODO: check FileEntity and physical file (in memory) are also deleted
        }

        public static async Task<ReadImageModel> Post_Should_Create_Image(HttpClient client, string name)
        {
            // arrange
            using var form = new MultipartFormDataFile(JpgImageBytes, name + ".jpg");

            // act
            using var response = await client.PostAsync(Path, form.MultipartFormDataContent);

            // assert
            var readModel = await Helper.VerifyAndGetAsync<ReadImageModel>(response, StatusCodes.Status201Created);
            Assert.True(readModel.Id > 0);
            Assert.Equal(name, readModel.Name);
            Assert.Equal(FileFormatEnum.JPG, readModel.Format);
            Assert.Equal(0, (int)readModel.Height);
            Assert.Equal(0, (int)readModel.Width);
            Assert.False(readModel.IsScanned);
            Assert.False(string.IsNullOrWhiteSpace(readModel.AbsoluteUrl));
            Assert.StartsWith("http://localhost/images/", readModel.AbsoluteUrl);
            Assert.EndsWith(".jpg", readModel.AbsoluteUrl);

            return readModel;
        }

        static async Task Get_Should_Return_One_Image(HttpClient client, ReadImageModel imageDto)
        {
            // arrange
            // act
            var images = await GetImagesAsync(client);

            // assert
            Assert.Equal(1, images?.Count);
            AssertImageDto(imageDto, images?[0]);
        }

        static async Task<ReadImageModel> Put_Should_Update_Image(HttpClient client, ReadImageModel model)
        {
            // arrange
            var updateModel = new UpdateImageModel("updated");

            // act
            using var response = await client.PutAsync(Path + $"/{model.Id}", Helper.GetContent(updateModel));

            // assert
            var readModel = await Helper.VerifyAndGetAsync<ReadImageModel>(response, StatusCodes.Status200OK);
            Assert.Equal(updateModel.Name, readModel.Name);
            Assert.Equal(model.Id, readModel.Id);
            Assert.NotEqual(model.Name, readModel.Name);
            var updated = model with { Name = updateModel.Name };
            AssertImageDto(updated, readModel);

            return readModel;
        }

        static async Task Get_Should_Return_Two_Images(HttpClient client, ReadImageModel imageDto, ReadImageModel imageDto2)
        {
            // arrange
            // act
            var images = await GetImagesAsync(client);

            // assert
            Assert.Equal(2, images?.Count);
            AssertImageDto(imageDto, images?[0]);
            AssertImageDto(imageDto2, images?[1]);
        }

        public static async Task Delete_Should_Delete_Image(HttpClient client, ReadImageModel model)
        {
            // arrange

            // act
            using var response = await client.DeleteAsync(Path + $"/{model.Id}");

            // assert
            Assert.Equal(StatusCodes.Status204NoContent, (int)response.StatusCode);
        }

        static async Task<IList<ReadImageModel>> GetImagesAsync(HttpClient client)
        {
            // arrange

            // act
            using var response = await client.GetAsync(Path);

            // assert
            var images = await Helper.VerifyAndGetAsync<IList<ReadImageModel>>(response, StatusCodes.Status200OK);

            return images;
        }

        public static async Task GetImageByIdAndAssertAsync(HttpClient client, ReadImageModel model)
        {
            // arrange

            // act
            using var response = await client.GetAsync(Path + "/" + model.Id);

            // assert
            var image = await Helper.VerifyAndGetAsync<ReadImageModel>(response, StatusCodes.Status200OK);

            AssertImageDto(model, image);
        }

        static void AssertImageDto(ReadImageModel expected, ReadImageModel? imageDto)
        {
            Assert.NotNull(imageDto);
            Assert.Equal(expected.Id, imageDto?.Id);
            Assert.Equal(expected.Name, imageDto?.Name);
            Assert.Equal(expected.AbsoluteUrl, imageDto?.AbsoluteUrl);
            Assert.Equal(expected.Format, imageDto?.Format);
            Assert.Equal(expected.Height, imageDto?.Height);
            Assert.Equal(expected.Width, imageDto?.Width);
        }
    }
}
