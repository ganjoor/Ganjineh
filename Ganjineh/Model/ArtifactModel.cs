using System;
using System.Collections.Generic;

namespace Ganjineh.Model
{
    public class ArtifactModel
    {
        public class RTag
        {
            public string id { get; set; }
            public int order { get; set; }
            public int tagType { get; set; }
            public string friendlyUrl { get; set; }
            public int status { get; set; }
            public string name { get; set; }
            public string nameInEnglish { get; set; }
            public string pluralName { get; set; }
            public string pluralNameInEnglish { get; set; }
            public bool globalValue { get; set; }
        }

        public class Value
        {
            public string id { get; set; }
            public string rTagId { get; set; }
            public RTag rTag { get; set; }
            public int order { get; set; }
            public string friendlyUrl { get; set; }
            public int status { get; set; }
            public string value { get; set; }
            public string valueInEnglish { get; set; }
            public string valueSupplement { get; set; }
        }

        public class ArtifactTag
        {
            public List<Value> values { get; set; }
            public string id { get; set; }
            public int order { get; set; }
            public int tagType { get; set; }
            public string friendlyUrl { get; set; }
            public int status { get; set; }
            public string name { get; set; }
            public string nameInEnglish { get; set; }
            public string pluralName { get; set; }
            public string pluralNameInEnglish { get; set; }
            public bool globalValue { get; set; }
        }

        public class RTagSum
        {
            public string tagName { get; set; }
            public string tagFriendlyUrl { get; set; }
            public int itemCount { get; set; }
        }

        public class Content
        {
            public string title { get; set; }
            public int order { get; set; }
            public int level { get; set; }
            public string itemFriendlyUrl { get; set; }
        }

        public class CoverImage
        {
            public string title { get; set; }
            public string titleInEnglish { get; set; }
            public string description { get; set; }
            public string descriptionInEnglish { get; set; }
            public int status { get; set; }
            public int order { get; set; }
            public string normalSizeImageStoredFileName { get; set; }
            public string thumbnailImageStoredFileName { get; set; }
            public int normalSizeImageWidth { get; set; }
            public int normalSizeImageHeight { get; set; }
            public int thumbnailImageWidth { get; set; }
            public int thumbnailImageHeight { get; set; }
            public string srcUrl { get; set; }
            public DateTime lastModifiedMeta { get; set; }
            public string id { get; set; }
            public string originalFileName { get; set; }
            public string contentType { get; set; }
            public int fileSizeInBytes { get; set; }
            public int imageWidth { get; set; }
            public int imageHeight { get; set; }
            public string folderName { get; set; }
            public string storedFileName { get; set; }
            public DateTime dataTime { get; set; }
            public DateTime lastModified { get; set; }
        }

        public class Image
        {
            public string title { get; set; }
            public string titleInEnglish { get; set; }
            public string description { get; set; }
            public string descriptionInEnglish { get; set; }
            public int status { get; set; }
            public int order { get; set; }
            public string normalSizeImageStoredFileName { get; set; }
            public string thumbnailImageStoredFileName { get; set; }
            public int normalSizeImageWidth { get; set; }
            public int normalSizeImageHeight { get; set; }
            public int thumbnailImageWidth { get; set; }
            public int thumbnailImageHeight { get; set; }
            public string srcUrl { get; set; }
            public DateTime lastModifiedMeta { get; set; }
            public string id { get; set; }
            public string originalFileName { get; set; }
            public string contentType { get; set; }
            public int fileSizeInBytes { get; set; }
            public int imageWidth { get; set; }
            public int imageHeight { get; set; }
            public string folderName { get; set; }
            public string storedFileName { get; set; }
            public DateTime dataTime { get; set; }
            public DateTime lastModified { get; set; }
        }

        public class Item
        {
            public string id { get; set; }
            public string rArtifactMasterRecordId { get; set; }
            public int order { get; set; }
            public string friendlyUrl { get; set; }
            public string name { get; set; }
            public string nameInEnglish { get; set; }
            public string description { get; set; }
            public string descriptionInEnglish { get; set; }
            public int coverImageIndex { get; set; }
            public DateTime lastModified { get; set; }
            public List<Image> images { get; set; }
            public object tags { get; set; }
        }

        public class RootObject
        {
            public List<ArtifactTag> artifactTags { get; set; }
            public List<RTagSum> rTagSums { get; set; }
            public List<Content> contents { get; set; }
            public string id { get; set; }
            public string friendlyUrl { get; set; }
            public int status { get; set; }
            public string name { get; set; }
            public string nameInEnglish { get; set; }
            public string description { get; set; }
            public string descriptionInEnglish { get; set; }
            public DateTime dateTime { get; set; }
            public DateTime lastModified { get; set; }
            public int coverItemIndex { get; set; }
            public CoverImage coverImage { get; set; }
            public string coverImageId { get; set; }
            public List<Item> items { get; set; }
            public int itemCount { get; set; }
            public object tags { get; set; }
        }
    }

    public class ContributorModel
    {
        public class Value
        {
            public string name { get; set; }
            public string friendlyUrl { get; set; }
            public int count { get; set; }
            public string imageId { get; set; }
        }

        public class RootObject
        {
            public string id { get; set; }
            public string friendlyUrl { get; set; }
            public string name { get; set; }
            public string pluralName { get; set; }
            public List<Value> values { get; set; }
        }
    }

    public class ContributorTagModel
    {
        public class CoverImage
        {
            public string title { get; set; }
            public string titleInEnglish { get; set; }
            public string description { get; set; }
            public string descriptionInEnglish { get; set; }
            public int status { get; set; }
            public int order { get; set; }
            public string normalSizeImageStoredFileName { get; set; }
            public string thumbnailImageStoredFileName { get; set; }
            public int normalSizeImageWidth { get; set; }
            public int normalSizeImageHeight { get; set; }
            public int thumbnailImageWidth { get; set; }
            public int thumbnailImageHeight { get; set; }
            public string srcUrl { get; set; }
            public DateTime lastModifiedMeta { get; set; }
            public string id { get; set; }
            public string originalFileName { get; set; }
            public string contentType { get; set; }
            public int fileSizeInBytes { get; set; }
            public int imageWidth { get; set; }
            public int imageHeight { get; set; }
            public string folderName { get; set; }
            public string storedFileName { get; set; }
            public DateTime dataTime { get; set; }
            public DateTime lastModified { get; set; }
        }

        public class RootObject
        {
            public string id { get; set; }
            public string friendlyUrl { get; set; }
            public int status { get; set; }
            public string name { get; set; }
            public string nameInEnglish { get; set; }
            public string description { get; set; }
            public string descriptionInEnglish { get; set; }
            public DateTime dateTime { get; set; }
            public DateTime lastModified { get; set; }
            public int coverItemIndex { get; set; }
            public CoverImage coverImage { get; set; }
            public string coverImageId { get; set; }
            public object items { get; set; }
            public int itemCount { get; set; }
            public object tags { get; set; }
        }
    }
}
