IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [Hotels] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [Address] nvarchar(500) NULL,
        [City] nvarchar(100) NOT NULL,
        [StarRating] int NOT NULL,
        [ContactName] nvarchar(100) NULL,
        [ContactPhone] nvarchar(50) NULL,
        [ContactEmail] nvarchar(255) NULL,
        [Images] nvarchar(max) NULL,
        [MinPrice] decimal(18,2) NULL,
        [MaxPrice] decimal(18,2) NULL,
        [Amenities] nvarchar(max) NULL,
        [CancellationPolicy] nvarchar(1000) NULL,
        [CheckInTime] nvarchar(500) NULL,
        [CheckOutTime] nvarchar(500) NULL,
        [Rating] float NOT NULL,
        [ReviewCount] int NOT NULL,
        [IsFeatured] bit NOT NULL,
        [Latitude] float NULL,
        [Longitude] float NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Hotels] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [Promotions] (
        [Id] uniqueidentifier NOT NULL,
        [Code] nvarchar(50) NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(1000) NULL,
        [DiscountType] int NOT NULL,
        [DiscountValue] decimal(18,2) NOT NULL,
        [MinOrderAmount] decimal(18,2) NULL,
        [MaxDiscountAmount] decimal(18,2) NULL,
        [StartDate] datetime2 NOT NULL,
        [EndDate] datetime2 NOT NULL,
        [ApplicableTo] nvarchar(max) NULL,
        [IsActive] bit NOT NULL,
        [TargetType] int NOT NULL,
        [TargetId] uniqueidentifier NULL,
        [ShowOnHome] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Promotions] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [Resorts] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [Address] nvarchar(500) NULL,
        [City] nvarchar(100) NOT NULL,
        [LocationType] nvarchar(100) NOT NULL,
        [StarRating] int NOT NULL,
        [Images] nvarchar(max) NULL,
        [MinPrice] decimal(18,2) NULL,
        [MaxPrice] decimal(18,2) NULL,
        [Amenities] nvarchar(max) NULL,
        [Activities] nvarchar(1000) NULL,
        [Packages] nvarchar(max) NULL,
        [Rating] float NOT NULL,
        [ReviewCount] int NOT NULL,
        [IsFeatured] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Resorts] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [Restaurants] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [Address] nvarchar(500) NULL,
        [City] nvarchar(100) NOT NULL,
        [CuisineType] nvarchar(max) NOT NULL,
        [PriceLevel] nvarchar(max) NOT NULL,
        [Style] nvarchar(max) NOT NULL,
        [OpeningTime] nvarchar(50) NULL,
        [ClosingTime] nvarchar(50) NULL,
        [ContactPhone] nvarchar(50) NULL,
        [Images] nvarchar(max) NULL,
        [Menu] nvarchar(max) NULL,
        [HasReservation] bit NOT NULL,
        [Rating] float NOT NULL,
        [ReviewCount] int NOT NULL,
        [IsFeatured] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Restaurants] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [Routes] (
        [RouteId] uniqueidentifier NOT NULL,
        [DepartureLocation] nvarchar(100) NOT NULL,
        [ArrivalLocation] nvarchar(100) NOT NULL,
        [RouteName] nvarchar(200) NULL,
        [Distance] float NULL,
        [Description] nvarchar(500) NULL,
        [EstimatedDuration] nvarchar(100) NULL,
        [Id] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Routes] PRIMARY KEY ([RouteId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [TourGuides] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Phone] nvarchar(20) NULL,
        [Email] nvarchar(255) NULL,
        [PhotoUrl] nvarchar(500) NULL,
        [Specialties] nvarchar(1000) NULL,
        [YearsExperience] int NOT NULL,
        [IsAvailable] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_TourGuides] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [TouristSpots] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [Region] nvarchar(100) NOT NULL,
        [Type] nvarchar(100) NOT NULL,
        [Address] nvarchar(500) NULL,
        [City] nvarchar(100) NULL,
        [Latitude] float NULL,
        [Longitude] float NULL,
        [Images] nvarchar(max) NULL,
        [TicketPrice] decimal(18,2) NULL,
        [BestTime] nvarchar(500) NULL,
        [Rating] float NOT NULL,
        [ReviewCount] int NOT NULL,
        [IsFeatured] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_TouristSpots] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [TourPackages] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(2000) NULL,
        [Destination] nvarchar(100) NOT NULL,
        [DurationDays] int NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [DiscountPrice] decimal(18,2) NULL,
        [Images] nvarchar(max) NULL,
        [Gallery] nvarchar(max) NULL,
        [Includes] nvarchar(max) NULL,
        [Excludes] nvarchar(max) NULL,
        [AvailableSlots] int NOT NULL,
        [DepartureDates] nvarchar(max) NULL,
        [Rating] float NOT NULL,
        [ReviewCount] int NOT NULL,
        [IsFeatured] bit NOT NULL,
        [IsNewArrival] bit NOT NULL,
        [IsHotDeal] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_TourPackages] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [Tours] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(3000) NULL,
        [DurationDays] int NOT NULL,
        [DurationNights] int NOT NULL,
        [BasePrice] decimal(18,2) NOT NULL,
        [Status] int NOT NULL,
        [ThumbnailUrl] nvarchar(500) NULL,
        [Highlights] nvarchar(1000) NULL,
        [Terms] nvarchar(500) NULL,
        [CancellationPolicy] nvarchar(500) NULL,
        [IsFeatured] bit NOT NULL,
        [IsDomestic] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Tours] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [TransportProviders] (
        [ProviderId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(500) NULL,
        [ContactPhone] nvarchar(50) NULL,
        [ContactEmail] nvarchar(100) NULL,
        [ContactAddress] nvarchar(200) NULL,
        [Website] nvarchar(100) NULL,
        [LogoUrl] nvarchar(500) NULL,
        [Id] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_TransportProviders] PRIMARY KEY ([ProviderId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [Transports] (
        [Id] uniqueidentifier NOT NULL,
        [Type] nvarchar(100) NOT NULL,
        [Provider] nvarchar(200) NOT NULL,
        [FromCity] nvarchar(100) NOT NULL,
        [ToCity] nvarchar(100) NOT NULL,
        [Route] nvarchar(200) NULL,
        [DepartureTime] time NULL,
        [ArrivalTime] time NULL,
        [DurationMinutes] int NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [AvailableSeats] int NOT NULL,
        [Amenities] nvarchar(max) NULL,
        [Images] nvarchar(max) NULL,
        [IsFeatured] bit NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Transports] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] uniqueidentifier NOT NULL,
        [Email] nvarchar(450) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [Avatar] nvarchar(max) NULL,
        [DateOfBirth] datetime2 NULL,
        [Gender] nvarchar(max) NULL,
        [TravelPreferences] nvarchar(max) NULL,
        [IsEmailVerified] bit NOT NULL,
        [Role] int NOT NULL,
        [IsLocked] bit NOT NULL,
        [RefreshToken] nvarchar(max) NULL,
        [RefreshTokenExpiryTime] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [VehicleTypes] (
        [VehicleTypeId] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [Icon] nvarchar(50) NULL,
        [Id] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_VehicleTypes] PRIMARY KEY ([VehicleTypeId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [HotelRooms] (
        [Id] uniqueidentifier NOT NULL,
        [RoomType] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [MaxOccupancy] int NOT NULL,
        [PricePerNight] decimal(18,2) NOT NULL,
        [BedType] nvarchar(max) NULL,
        [RoomAmenities] nvarchar(max) NULL,
        [Images] nvarchar(max) NULL,
        [TotalRooms] int NOT NULL,
        [AvailableRooms] int NOT NULL,
        [HotelId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_HotelRooms] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HotelRooms_Hotels_HotelId] FOREIGN KEY ([HotelId]) REFERENCES [Hotels] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [ResortRooms] (
        [Id] uniqueidentifier NOT NULL,
        [RoomType] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [MaxOccupancy] int NOT NULL,
        [PricePerNight] decimal(18,2) NOT NULL,
        [BedType] nvarchar(max) NULL,
        [RoomAmenities] nvarchar(max) NULL,
        [Images] nvarchar(max) NULL,
        [TotalRooms] int NOT NULL,
        [AvailableRooms] int NOT NULL,
        [ResortId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ResortRooms] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ResortRooms_Resorts_ResortId] FOREIGN KEY ([ResortId]) REFERENCES [Resorts] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [TourDepartures] (
        [Id] uniqueidentifier NOT NULL,
        [DepartureDate] datetime2 NOT NULL,
        [AvailableSeats] int NOT NULL,
        [TotalSeats] int NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [DiscountPrice] decimal(18,2) NULL,
        [IsAvailable] bit NOT NULL,
        [Notes] nvarchar(max) NULL,
        [TourId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_TourDepartures] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TourDepartures_Tours_TourId] FOREIGN KEY ([TourId]) REFERENCES [Tours] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [TourDestinations] (
        [Id] uniqueidentifier NOT NULL,
        [DisplayOrder] int NOT NULL,
        [TourId] uniqueidentifier NOT NULL,
        [TouristSpotId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_TourDestinations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TourDestinations_TouristSpots_TouristSpotId] FOREIGN KEY ([TouristSpotId]) REFERENCES [TouristSpots] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_TourDestinations_Tours_TourId] FOREIGN KEY ([TourId]) REFERENCES [Tours] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [TourImages] (
        [Id] uniqueidentifier NOT NULL,
        [ImageUrl] nvarchar(500) NOT NULL,
        [Caption] nvarchar(200) NULL,
        [DisplayOrder] int NOT NULL,
        [IsPrimary] bit NOT NULL,
        [TourId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_TourImages] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TourImages_Tours_TourId] FOREIGN KEY ([TourId]) REFERENCES [Tours] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [TourItineraries] (
        [Id] uniqueidentifier NOT NULL,
        [DayNumber] int NOT NULL,
        [Title] nvarchar(200) NOT NULL,
        [Content] nvarchar(max) NULL,
        [Meals] nvarchar(1000) NULL,
        [Accommodation] nvarchar(500) NULL,
        [Transport] nvarchar(500) NULL,
        [Activities] nvarchar(1000) NULL,
        [Notes] nvarchar(max) NULL,
        [TourId] uniqueidentifier NOT NULL,
        [TourPackageId] uniqueidentifier NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_TourItineraries] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TourItineraries_TourPackages_TourPackageId] FOREIGN KEY ([TourPackageId]) REFERENCES [TourPackages] ([Id]),
        CONSTRAINT [FK_TourItineraries_Tours_TourId] FOREIGN KEY ([TourId]) REFERENCES [Tours] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [TourServices] (
        [Id] uniqueidentifier NOT NULL,
        [ServiceName] nvarchar(200) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsIncluded] bit NOT NULL,
        [Category] int NOT NULL,
        [TourId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_TourServices] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TourServices_Tours_TourId] FOREIGN KEY ([TourId]) REFERENCES [Tours] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [TourTourGuides] (
        [TourGuidesId] uniqueidentifier NOT NULL,
        [ToursId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_TourTourGuides] PRIMARY KEY ([TourGuidesId], [ToursId]),
        CONSTRAINT [FK_TourTourGuides_TourGuides_TourGuidesId] FOREIGN KEY ([TourGuidesId]) REFERENCES [TourGuides] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_TourTourGuides_Tours_ToursId] FOREIGN KEY ([ToursId]) REFERENCES [Tours] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [AccountActivities] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [Action] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IPAddress] nvarchar(50) NULL,
        [UserAgent] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_AccountActivities] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AccountActivities_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [Addresses] (
        [Id] uniqueidentifier NOT NULL,
        [AddressLine] nvarchar(500) NOT NULL,
        [Ward] nvarchar(100) NULL,
        [District] nvarchar(100) NULL,
        [City] nvarchar(100) NOT NULL,
        [Country] nvarchar(100) NOT NULL,
        [IsDefault] bit NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Addresses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Addresses_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [Contacts] (
        [Id] uniqueidentifier NOT NULL,
        [FullName] nvarchar(100) NOT NULL,
        [Email] nvarchar(255) NOT NULL,
        [PhoneNumber] nvarchar(20) NULL,
        [Address] nvarchar(500) NULL,
        [Subject] nvarchar(200) NULL,
        [RequestType] int NOT NULL,
        [ServiceType] nvarchar(100) NULL,
        [ExpectedDate] datetime2 NULL,
        [ParticipantCount] int NULL,
        [MessageContent] nvarchar(max) NOT NULL,
        [Rating] int NULL,
        [Status] int NOT NULL,
        [ReplyMessage] nvarchar(1000) NULL,
        [RepliedAt] datetime2 NULL,
        [UserId] uniqueidentifier NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Contacts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Contacts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE SET NULL
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [Favorites] (
        [Id] uniqueidentifier NOT NULL,
        [ItemType] int NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [TouristSpotId] uniqueidentifier NULL,
        [HotelId] uniqueidentifier NULL,
        [RestaurantId] uniqueidentifier NULL,
        [ResortId] uniqueidentifier NULL,
        [TourPackageId] uniqueidentifier NULL,
        [TransportId] uniqueidentifier NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Favorites] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Favorites_Hotels_HotelId] FOREIGN KEY ([HotelId]) REFERENCES [Hotels] ([Id]),
        CONSTRAINT [FK_Favorites_Resorts_ResortId] FOREIGN KEY ([ResortId]) REFERENCES [Resorts] ([Id]),
        CONSTRAINT [FK_Favorites_Restaurants_RestaurantId] FOREIGN KEY ([RestaurantId]) REFERENCES [Restaurants] ([Id]),
        CONSTRAINT [FK_Favorites_TourPackages_TourPackageId] FOREIGN KEY ([TourPackageId]) REFERENCES [TourPackages] ([Id]),
        CONSTRAINT [FK_Favorites_TouristSpots_TouristSpotId] FOREIGN KEY ([TouristSpotId]) REFERENCES [TouristSpots] ([Id]),
        CONSTRAINT [FK_Favorites_Transports_TransportId] FOREIGN KEY ([TransportId]) REFERENCES [Transports] ([Id]),
        CONSTRAINT [FK_Favorites_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [SearchHistories] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [SearchQuery] nvarchar(200) NOT NULL,
        [Category] nvarchar(50) NOT NULL,
        [SearchDate] datetime2 NOT NULL,
        [Location] nvarchar(100) NULL,
        [ResultCount] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_SearchHistories] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SearchHistories_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [Vehicles] (
        [VehicleId] uniqueidentifier NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [LicensePlate] nvarchar(50) NOT NULL,
        [Capacity] int NOT NULL,
        [VehicleTypeId] uniqueidentifier NOT NULL,
        [ProviderId] uniqueidentifier NOT NULL,
        [Description] nvarchar(500) NULL,
        [ImageUrl] nvarchar(500) NULL,
        [Amenities] nvarchar(100) NULL,
        [Status] int NOT NULL,
        [Id] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Vehicles] PRIMARY KEY ([VehicleId]),
        CONSTRAINT [FK_Vehicles_TransportProviders_ProviderId] FOREIGN KEY ([ProviderId]) REFERENCES [TransportProviders] ([ProviderId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Vehicles_VehicleTypes_VehicleTypeId] FOREIGN KEY ([VehicleTypeId]) REFERENCES [VehicleTypes] ([VehicleTypeId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [HotelRoomAvailabilities] (
        [Id] uniqueidentifier NOT NULL,
        [RoomId] uniqueidentifier NOT NULL,
        [Date] datetime2 NOT NULL,
        [AvailableRooms] int NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_HotelRoomAvailabilities] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HotelRoomAvailabilities_HotelRooms_RoomId] FOREIGN KEY ([RoomId]) REFERENCES [HotelRooms] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [Bookings] (
        [Id] uniqueidentifier NOT NULL,
        [BookingCode] nvarchar(50) NOT NULL,
        [Type] int NOT NULL,
        [Status] int NOT NULL,
        [ServiceDate] datetime2 NULL,
        [EndDate] datetime2 NULL,
        [Quantity] int NOT NULL,
        [TotalAmount] decimal(18,2) NOT NULL,
        [DiscountAmount] decimal(18,2) NULL,
        [FinalAmount] decimal(18,2) NOT NULL,
        [PaymentStatus] int NOT NULL,
        [PaymentMethod] nvarchar(50) NULL,
        [PaidAt] datetime2 NULL,
        [Notes] nvarchar(500) NULL,
        [CancellationReason] nvarchar(500) NULL,
        [CancelledAt] datetime2 NULL,
        [ExpiredAt] datetime2 NULL,
        [ContactName] nvarchar(100) NOT NULL,
        [ContactEmail] nvarchar(255) NOT NULL,
        [ContactPhone] nvarchar(20) NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [TourId] uniqueidentifier NULL,
        [TourPackageId] uniqueidentifier NULL,
        [HotelId] uniqueidentifier NULL,
        [HotelRoomId] uniqueidentifier NULL,
        [ResortId] uniqueidentifier NULL,
        [ResortRoomId] uniqueidentifier NULL,
        [TransportId] uniqueidentifier NULL,
        [PromotionId] uniqueidentifier NULL,
        [RestaurantId] uniqueidentifier NULL,
        [TourDepartureId] uniqueidentifier NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Bookings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Bookings_HotelRooms_HotelRoomId] FOREIGN KEY ([HotelRoomId]) REFERENCES [HotelRooms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_Hotels_HotelId] FOREIGN KEY ([HotelId]) REFERENCES [Hotels] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_Promotions_PromotionId] FOREIGN KEY ([PromotionId]) REFERENCES [Promotions] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_ResortRooms_ResortRoomId] FOREIGN KEY ([ResortRoomId]) REFERENCES [ResortRooms] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_Resorts_ResortId] FOREIGN KEY ([ResortId]) REFERENCES [Resorts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_Restaurants_RestaurantId] FOREIGN KEY ([RestaurantId]) REFERENCES [Restaurants] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_TourDepartures_TourDepartureId] FOREIGN KEY ([TourDepartureId]) REFERENCES [TourDepartures] ([Id]),
        CONSTRAINT [FK_Bookings_TourPackages_TourPackageId] FOREIGN KEY ([TourPackageId]) REFERENCES [TourPackages] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_Tours_TourId] FOREIGN KEY ([TourId]) REFERENCES [Tours] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_Transports_TransportId] FOREIGN KEY ([TransportId]) REFERENCES [Transports] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Bookings_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [Schedules] (
        [ScheduleId] uniqueidentifier NOT NULL,
        [VehicleId] uniqueidentifier NOT NULL,
        [RouteId] uniqueidentifier NOT NULL,
        [DepartureTime] time NOT NULL,
        [ArrivalTime] time NULL,
        [Price] decimal(18,2) NOT NULL,
        [AvailableSeats] int NOT NULL,
        [TotalSeats] int NOT NULL,
        [OperatingDays] nvarchar(max) NULL,
        [Notes] nvarchar(100) NULL,
        [Status] int NOT NULL,
        [Id] uniqueidentifier NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Schedules] PRIMARY KEY ([ScheduleId]),
        CONSTRAINT [FK_Schedules_Routes_RouteId] FOREIGN KEY ([RouteId]) REFERENCES [Routes] ([RouteId]) ON DELETE CASCADE,
        CONSTRAINT [FK_Schedules_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([VehicleId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE TABLE [Reviews] (
        [Id] uniqueidentifier NOT NULL,
        [Rating] int NOT NULL,
        [Title] nvarchar(200) NULL,
        [Content] nvarchar(2000) NULL,
        [Images] nvarchar(max) NULL,
        [Type] int NOT NULL,
        [UserId] uniqueidentifier NOT NULL,
        [TouristSpotId] uniqueidentifier NULL,
        [HotelId] uniqueidentifier NULL,
        [RestaurantId] uniqueidentifier NULL,
        [ResortId] uniqueidentifier NULL,
        [TourPackageId] uniqueidentifier NULL,
        [BookingId] uniqueidentifier NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [DeletedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Reviews_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE SET NULL,
        CONSTRAINT [FK_Reviews_Hotels_HotelId] FOREIGN KEY ([HotelId]) REFERENCES [Hotels] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Reviews_Resorts_ResortId] FOREIGN KEY ([ResortId]) REFERENCES [Resorts] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Reviews_Restaurants_RestaurantId] FOREIGN KEY ([RestaurantId]) REFERENCES [Restaurants] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Reviews_TourPackages_TourPackageId] FOREIGN KEY ([TourPackageId]) REFERENCES [TourPackages] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Reviews_TouristSpots_TouristSpotId] FOREIGN KEY ([TouristSpotId]) REFERENCES [TouristSpots] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Reviews_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_AccountActivities_Action] ON [AccountActivities] ([Action]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_AccountActivities_CreatedAt] ON [AccountActivities] ([CreatedAt]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_AccountActivities_UserId] ON [AccountActivities] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Addresses_UserId] ON [Addresses] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Bookings_BookingCode] ON [Bookings] ([BookingCode]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Bookings_HotelId] ON [Bookings] ([HotelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Bookings_HotelRoomId] ON [Bookings] ([HotelRoomId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Bookings_PromotionId] ON [Bookings] ([PromotionId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Bookings_ResortId] ON [Bookings] ([ResortId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Bookings_ResortRoomId] ON [Bookings] ([ResortRoomId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Bookings_RestaurantId] ON [Bookings] ([RestaurantId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Bookings_Status] ON [Bookings] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Bookings_TourDepartureId] ON [Bookings] ([TourDepartureId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Bookings_TourId] ON [Bookings] ([TourId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Bookings_TourPackageId] ON [Bookings] ([TourPackageId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Bookings_TransportId] ON [Bookings] ([TransportId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Bookings_UserId] ON [Bookings] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Contacts_Status] ON [Contacts] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Contacts_UserId] ON [Contacts] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Favorites_HotelId] ON [Favorites] ([HotelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Favorites_ResortId] ON [Favorites] ([ResortId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Favorites_RestaurantId] ON [Favorites] ([RestaurantId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Favorites_TouristSpotId] ON [Favorites] ([TouristSpotId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Favorites_TourPackageId] ON [Favorites] ([TourPackageId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Favorites_TransportId] ON [Favorites] ([TransportId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Favorites_UserId] ON [Favorites] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_HotelRoomAvailabilities_RoomId] ON [HotelRoomAvailabilities] ([RoomId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_HotelRooms_HotelId] ON [HotelRooms] ([HotelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Hotels_City] ON [Hotels] ([City]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Hotels_Name] ON [Hotels] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Promotions_Code] ON [Promotions] ([Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_ResortRooms_ResortId] ON [ResortRooms] ([ResortId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Resorts_City] ON [Resorts] ([City]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Resorts_Name] ON [Resorts] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Restaurants_City] ON [Restaurants] ([City]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Restaurants_Name] ON [Restaurants] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Reviews_BookingId] ON [Reviews] ([BookingId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Reviews_HotelId] ON [Reviews] ([HotelId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Reviews_ResortId] ON [Reviews] ([ResortId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Reviews_RestaurantId] ON [Reviews] ([RestaurantId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Reviews_TouristSpotId] ON [Reviews] ([TouristSpotId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Reviews_TourPackageId] ON [Reviews] ([TourPackageId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Reviews_Type] ON [Reviews] ([Type]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Reviews_UserId] ON [Reviews] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Routes_ArrivalLocation] ON [Routes] ([ArrivalLocation]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Routes_DepartureLocation] ON [Routes] ([DepartureLocation]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Schedules_DepartureTime] ON [Schedules] ([DepartureTime]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Schedules_RouteId] ON [Schedules] ([RouteId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Schedules_Status] ON [Schedules] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Schedules_VehicleId] ON [Schedules] ([VehicleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_SearchHistories_UserId] ON [SearchHistories] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TourDepartures_TourId_DepartureDate] ON [TourDepartures] ([TourId], [DepartureDate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TourDestinations_TourId_TouristSpotId] ON [TourDestinations] ([TourId], [TouristSpotId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_TourDestinations_TouristSpotId] ON [TourDestinations] ([TouristSpotId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_TourGuides_Email] ON [TourGuides] ([Email]) WHERE [Email] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_TourImages_TourId] ON [TourImages] ([TourId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_TouristSpots_City] ON [TouristSpots] ([City]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_TouristSpots_Name] ON [TouristSpots] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE UNIQUE INDEX [IX_TourItineraries_TourId_DayNumber] ON [TourItineraries] ([TourId], [DayNumber]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_TourItineraries_TourPackageId] ON [TourItineraries] ([TourPackageId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_TourPackages_Destination] ON [TourPackages] ([Destination]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_TourPackages_Name] ON [TourPackages] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Tours_Name] ON [Tours] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Tours_Status] ON [Tours] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_TourServices_TourId] ON [TourServices] ([TourId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_TourTourGuides_ToursId] ON [TourTourGuides] ([ToursId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_TransportProviders_Name] ON [TransportProviders] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Transports_FromCity] ON [Transports] ([FromCity]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Transports_ToCity] ON [Transports] ([ToCity]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Transports_Type] ON [Transports] ([Type]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Vehicles_LicensePlate] ON [Vehicles] ([LicensePlate]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Vehicles_Name] ON [Vehicles] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Vehicles_ProviderId] ON [Vehicles] ([ProviderId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Vehicles_Status] ON [Vehicles] ([Status]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_Vehicles_VehicleTypeId] ON [Vehicles] ([VehicleTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    CREATE INDEX [IX_VehicleTypes_Name] ON [VehicleTypes] ([Name]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260315164318_AddProfileFields'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260315164318_AddProfileFields', N'8.0.0');
END;
GO

COMMIT;
GO

