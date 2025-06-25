using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectsComparer;
using Comparer = ObjectsComparer.Comparer;

namespace AtCommon.Utilities
{
    public static class AssertExtensions
    {

        public static void ImagesAreEqual(this Assert _, string expectedImage, string actualImage,
            string message = null)
        {
            var image1 = new Bitmap(expectedImage);
            var image2 = new Bitmap(actualImage);

            var hashA = CSharpHelpers.GetImageHash(image1);
            var hashB = CSharpHelpers.GetImageHash(image2);

            if (hashA.Where((nextByte, index) => nextByte != hashB[index]).Any())
                Assert.Fail(message ?? "Image does not match");

        }

        public static void ImageSizesAreEqual(this Assert _, string expectedImage, string actualImage,
            string message = null)
        {
            var image1 = new Bitmap(expectedImage);
            var image2 = new Bitmap(actualImage);

            if (image1.Size != image2.Size)
                Assert.Fail(message ??
                            $"Size does not match. Expected: <{image1.Size}> Actual: <{image2.Size}>");

        }

        public static void ExcelAreEqual(this Assert _, string expectedFilePath, string actualFilePath)
        {
            var worksheets = ExcelUtil.GetWorkSheetNames(expectedFilePath);
            foreach (var sheet in worksheets)
            {
                var actualTable = ExcelUtil.GetExcelData(actualFilePath, sheet);
                var expectedTable = ExcelUtil.GetExcelData(expectedFilePath, sheet);

                // verify columns
                var actualColumns = (from DataColumn item in actualTable.Columns select item.ColumnName).ToList();
                var expectedColumns = (from DataColumn item in expectedTable.Columns select item.ColumnName).ToList();
                Assert.That.ListsAreEqual(expectedColumns, actualColumns);

                // verify rows
                for (var i = 0; i < expectedTable.Rows.Count; i++)
                {
                    var expectedRow = expectedTable.Rows[i].ItemArray.Select(item => item.ToString()).ToList();
                    var actualRow = actualTable.Rows[i].ItemArray.Select(item => item.ToString()).ToList();

                    Assert.AreEqual(expectedRow.Count, actualRow.Count, $"Row {i} cell count does not match");

                    for (var j = 0; j < expectedRow.Count; j++)
                    {
                        Assert.AreEqual(expectedRow[j], actualRow[j], $"Row {i}, Column {j} value doesn't match");

                    }
                } 
            }
        }

        public static void ListContains(this Assert _, IList list, object value, string message = "")
        {
            Assert.IsTrue(list.Contains(value), 
                $"{message} Expected: <{value}>. Actual: <{string.Join(", ", list.Cast<string>())}>");
        }

        public static void ListNotContains(this Assert _, IList list, object value, string message = "")
        {
            Assert.IsFalse(list.Contains(value), 
                $"{message} Expected: <{value}>. Actual: <{string.Join(", ", list.Cast<string>())}>");
        }

        public static void ListsAreEqual(this Assert _, IList expected, IList actual, string message = "")
        {
            var missing = expected.Cast<string>().Except(actual.Cast<string>()).ToList();
            var extra = actual.Cast<string>().Except(expected.Cast<string>()).ToList();

            if (!missing.Any() && !extra.Any()) return;

            var failMessage = message == "" ? "Lists are not equal. " : message + " ";
            if (missing.Any()) { failMessage += $"Missing items: <{string.Join(",", missing)}>. "; }
            if (extra.Any()) { failMessage += $"Extra items: <{string.Join(",", extra)}>. "; }

            Assert.Fail(failMessage);
        }

        public static void TimeIsClose(this Assert _, DateTime expected, DateTime actual, int maxMinutesDeviation = 2, string message = "")
        {
            var datetimeDifference = actual.Subtract(expected);
            Assert.IsTrue(
                datetimeDifference < TimeSpan.FromMinutes(maxMinutesDeviation) &&
                datetimeDifference > TimeSpan.FromMinutes(maxMinutesDeviation * -1),
                $"{message} Time is not within allowed limit of {maxMinutesDeviation} minutes - Expected:<{expected:g}>. Actual:<{actual:g}>.");
        }

        public static void ResponseAreEqual(this Assert _, dynamic expected, dynamic actual, List<string> ignorePropertiesList = null)
        {
            var comparer = new Comparer();

            if (ignorePropertiesList != null)
            {
                foreach (var property in ignorePropertiesList)
                {
                    comparer.IgnoreMember(property);
                }
            }
            
            var compareExpectedActualResponse = comparer.Compare(expected, actual, out IEnumerable<Difference> differences);

            var differencesList = differences.ToList();
            
            try
            {
                Assert.IsTrue(compareExpectedActualResponse);
            }
            catch (Exception)
            {
                var errorMessages = differencesList.Select(diff => $"\n`{diff.MemberPath}` does not match. Expected value was `{diff.Value1}` and actual value is `{diff.Value2}`").ToList();
                var messages = string.Join("\n", errorMessages);
                Assert.Fail(messages);
            }
        }
    }
}