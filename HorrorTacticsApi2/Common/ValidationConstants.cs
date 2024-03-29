﻿namespace HorrorTacticsApi2.Common
{
    public static class ValidationConstants
    {
        public const string RegularExpressionForAllStrings = @"^[\r\n\P{Cc}]*$";
        public const int RegularExpressionTimeoutMilliseconds = 2500;

        public const int File_Name_MaxStringLength = 300;
        public const int File_Name_MinStringLength = 1;

        public const int File_Filename_MaxStringLength = 100;
        public const int File_Filename_MinStringLength = 1;

        public const int StoryScene_Text_MinStringLength = 1;
        public const int StoryScene_Text_MaxStringLength = 1000;

        public const int Story_Title_MaxStringLength = 300;
        public const int Story_Title_MinStringLength = 1;
        public const int Story_Description_MaxStringLength = 600;

        public const int StoryScene_Title_MaxStringLength = 60;
        public const int StoryScene_Title_MinStringLength = 1;

        public const int StorySceneCommand_Title_MaxStringLength = 60;
        public const int StorySceneCommand_Title_MinStringLength = 1;

        public const int StorySceneCommand_Comments_MaxStringLength = 600;

        public const int StoryScene_Items_List_MaxLength = 50;

        public const int User_Username_MaxLength = 50;
        public const int User_Username_MinLength = 1;

        public const int User_Password_MaxLength = 50;
        public const int User_Password_MinLength = 5;

        public const int Game_Notes_MaxStringLength = 5000;
    }
}
