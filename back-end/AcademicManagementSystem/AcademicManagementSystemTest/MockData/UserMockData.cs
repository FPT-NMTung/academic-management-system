﻿using AcademicManagementSystem.Context.AmsModels;

namespace AcademicManagementSystemTest.MockData;

public static class UserMockData
{
    public static List<User> Users = new List<User>
    {
        new User
        {
            Id = 1,
            ProvinceId = 1,
            DistrictId = 25,
            WardId = 0,
            CenterId = 1,
            GenderId = 1,
            RoleId = 1,
            FirstName = "Nguyễn Mạnh",
            LastName = "Tùng",
            Avatar = "https://photos.nmtung.dev/ams/avatar/user-id-2-20221123124123.png",
            MobilePhone = "0853576259",
            Email = "admin@nmtung.dev",
            EmailOrganization = "nmtung.study@gmail.com",
            Birthday = new DateTime(2000, 11, 13),
            CitizenIdentityCardNo = "111111111111",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 11, 13),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsActive = true
        },
        new User
        {
            Id = 2,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            RoleId = 1,
            FirstName = "Minh",
            LastName = "Thành Admin",
            Avatar = null,
            MobilePhone = "0985563540",
            Email = "nmthanh1306@gmail.com",
            EmailOrganization = "nmthanh1306@gmail.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "1234567890",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsActive = true
        },
        new User
        {
            Id = 3,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            RoleId = 2,
            FirstName = "Minh",
            LastName = "Thành SRO",
            Avatar = null,
            MobilePhone = "0985563541",
            Email = "thanhnm136@gmail.com",
            EmailOrganization = "thanhnm136@gmail.com",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "1234567891",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsActive = true
        },
        new User
        {
            Id = 4,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            RoleId = 3,
            FirstName = "Minh",
            LastName = "Thành Teacher",
            Avatar = null,
            MobilePhone = "0985563542",
            Email = "thanhnmhe141011@fpt.edu.vn",
            EmailOrganization = "thanhnmhe141011@fpt.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "1234567892",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsActive = true
        },
        new User
        {
            Id = 5,
            ProvinceId = 1,
            DistrictId = 1,
            WardId = 1,
            CenterId = 1,
            GenderId = 1,
            RoleId = 4,
            FirstName = "Minh",
            LastName = "Thành Student",
            Avatar = null,
            MobilePhone = "0985563543",
            Email = "thanhnm_student@fpt.edu.vn",
            EmailOrganization = "thanhnm_student@fpt.edu.vn",
            Birthday = new DateTime(2000, 01, 01),
            CitizenIdentityCardNo = "1234567893",
            CitizenIdentityCardPublishedDate = new DateTime(2019, 01, 01),
            CitizenIdentityCardPublishedPlace = "Hà Nội",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            IsActive = true
        }
    };
}