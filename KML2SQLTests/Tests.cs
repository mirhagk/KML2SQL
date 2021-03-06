﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KML2SQL;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System.Diagnostics;
using System.Text;

namespace KML2SQLTests
{
    [TestClass]
    public class Tests
    {
        //====================================================================================================
        //
        // Yes, I know these aren't real tests. Sorry. I wasn't really doing TDD at the time I wrote this.
        //
        //====================================================================================================

        private const string UserInfoLocation = @"C:\Misc\UserInfo.json";
        readonly string _settingsText = File.ReadAllText(UserInfoLocation);
        private UserInfo _userInfo;

        MapUploader myUploader;
        Kml kml;
        private string connectionString;

        [TestInitialize]
        public void InitializeTests()
        {
            _userInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<UserInfo>(_settingsText);
            connectionString = "Data Source=" + _userInfo.Server + ";Initial Catalog=" + _userInfo.Database +
                               ";Persist Security Info=True;User ID="
                               + _userInfo.Username + ";Password=" + _userInfo.Password;
            myUploader = new MapUploader(connectionString);
        }

        [TestMethod]
        public void CheckNPA()
        {
            myUploader.Upload("polygon", @"TestData\npa.kml", _userInfo.Table, 4326, true);
        }

        [TestMethod]
        public void BasicKML()
        {
            myUploader.Upload( "polygon", @"TestData\Basic.kml", _userInfo.Table, 4326, true);
        }

        [TestMethod]
        public void BasicKMLGeometry()
        {
            myUploader.Upload("polygon", @"TestData\Basic.kml", _userInfo.Table, 4326, false);
        }

        [TestMethod]
        public void CheckNPAGeometry()
        {
            myUploader.Upload("polygon", @"TestData\npa.kml", _userInfo.Table, 4326, false);
        }

        [TestMethod]
        public void SchoolTest()
        {
            myUploader.Upload( "polygon", @"TestData\school.kml", _userInfo.Table, 4326, true);
        }

        [TestMethod]
        public void SchoolTestGeometry()
        {
            myUploader.Upload("polygon", @"TestData\school.kml", _userInfo.Table, 4326, false);
        }

        [TestMethod]
        public void GoogleSample()
        {
            myUploader.Upload("polygon", @"TestData\KML_Samples.kml", _userInfo.Table, 4326, false);
        }

        //[TestMethod]
        //public void BasicKmlOnMySql()
        //{
        //    myUploader = new MapUploader("192.168.0.202", "test", "root", passwordList[1], "placemark", @"TestData\Basic.kml", myInfo.Table, 4326, true);
        //    myUploader.Upload();
        //}
    }
}
