﻿var PotentialCustomerModel = function () {
    this.Id = ko.observable("");
    this.Code = ko.observable("");
    this.Email = ko.observable("");
    this.FirstName = ko.observable("");
    this.LastName = ko.observable("");
    this.MidleName = ko.observable("");
    this.FullName = ko.observable("");
    this.SystemId = ko.observable("");
    this.SystemName = ko.observable("");
    this.Phone = ko.observable("");
    this.Avatar = ko.observable("");
    this.Nickname = ko.observable("");
    this.Birthday = ko.observable("");
    this.LevelId = ko.observable("");
    this.LevelName = ko.observable("");
    this.GenderId = ko.observable("");
    this.GenderName = ko.observable(-1);
    this.DistrictId = ko.observable("");
    this.DistrictName = ko.observable("");
    this.ProvinceId = ko.observable("");
    this.ProvinceName = ko.observable("");
    this.WardId = ko.observable("");
    this.WardsName = ko.observable("");
    this.Address = ko.observable("");
    this.UserId = ko.observable("");
    this.UserFullName = ko.observable("");
    this.Created = ko.observable("");
    this.Updated = ko.observable("");
    this.HashTag = ko.observable("");
    this.CountryId = ko.observable(""); 
    this.IsDelete = ko.observable("");
    this.CustomerTypeId = ko.observable("");
    this.CustomerTypeName = ko.observable("");
};