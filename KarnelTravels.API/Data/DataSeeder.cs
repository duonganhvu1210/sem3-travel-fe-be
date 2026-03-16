using KarnelTravels.API.Entities;
using Microsoft.EntityFrameworkCore;
using Route = KarnelTravels.API.Entities.Route;

namespace KarnelTravels.API.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(KarnelTravelsDbContext context)
    {
        // Apply migrations first
        await context.Database.MigrateAsync();

        // Check if data already exists - only seed if empty
        if (await context.Users.AnyAsync())
        {
            Console.WriteLine("Database already seeded, skipping...");
            return;
        }

        // Only clear and seed base data - NOT bookings (keep existing bookings)
        context.Addresses.RemoveRange(context.Addresses);
        context.TouristSpots.RemoveRange(context.TouristSpots);
        context.Hotels.RemoveRange(context.Hotels);
        context.HotelRooms.RemoveRange(context.HotelRooms);
        context.Restaurants.RemoveRange(context.Restaurants);
        context.Resorts.RemoveRange(context.Resorts);
        context.ResortRooms.RemoveRange(context.ResortRooms);
        context.Transports.RemoveRange(context.Transports);
        context.TourPackages.RemoveRange(context.TourPackages);
        context.Tours.RemoveRange(context.Tours);
        context.Promotions.RemoveRange(context.Promotions);
        context.Reviews.RemoveRange(context.Reviews);
        context.Favorites.RemoveRange(context.Favorites);
        context.Contacts.RemoveRange(context.Contacts);
        context.TourGuides.RemoveRange(context.TourGuides);
        context.Vehicles.RemoveRange(context.Vehicles);
        context.Routes.RemoveRange(context.Routes);
        context.Schedules.RemoveRange(context.Schedules);
        // DO NOT remove bookings - they should persist
        await context.SaveChangesAsync();

        // ==================== USERS ====================
        var adminUser = new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Email = "admin@karneltravels.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            FullName = "Admin System",
            PhoneNumber = "0901234567",
            Avatar = "https://i.pravatar.cc/150?img=1",
            Role = UserRole.Admin,
            IsEmailVerified = true
        };

        var managerUser = new User
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Email = "manager@karneltravels.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123"),
            FullName = "Nguyễn Văn Manager",
            PhoneNumber = "0902234567",
            Avatar = "https://i.pravatar.cc/150?img=2",
            Role = UserRole.Manager,
            IsEmailVerified = true
        };

        var staffUser = new User
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Email = "staff@karneltravels.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("staff123"),
            FullName = "Trần Thị Staff",
            PhoneNumber = "0903234567",
            Avatar = "https://i.pravatar.cc/150?img=3",
            Role = UserRole.Staff,
            IsEmailVerified = true
        };

        var users = new List<User>
        {
            adminUser,
            managerUser,
            staffUser,
            new User
            {
                Email = "john.doe@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                FullName = "John Doe",
                PhoneNumber = "0904234567",
                Avatar = "https://i.pravatar.cc/150?img=4",
                Role = UserRole.User,
                IsEmailVerified = true
            },
            new User
            {
                Email = "jane.smith@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                FullName = "Jane Smith",
                PhoneNumber = "0905234567",
                Avatar = "https://i.pravatar.cc/150?img=5",
                Role = UserRole.User,
                IsEmailVerified = true
            },
            new User
            {
                Email = "minh.chau@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                FullName = "Lê Minh Châu",
                PhoneNumber = "0906234567",
                Avatar = "https://i.pravatar.cc/150?img=6",
                Role = UserRole.User,
                IsEmailVerified = true
            },
            new User
            {
                Email = "tuan.anh@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                FullName = "Phạm Tuấn Anh",
                PhoneNumber = "0907234567",
                Avatar = "https://i.pravatar.cc/150?img=7",
                Role = UserRole.User,
                IsEmailVerified = true
            },
            new User
            {
                Email = "thao.linh@email.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                FullName = "Đỗ Thảo Linh",
                PhoneNumber = "0908234567",
                Avatar = "https://i.pravatar.cc/150?img=8",
                Role = UserRole.User,
                IsEmailVerified = true
            }
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        // ==================== ADDRESSES ====================
        var addresses = new List<Address>
        {
            new Address
            {
                AddressLine = "123 Đường Nguyễn Trãi",
                Ward = "Phường 1",
                District = "Quận 1",
                City = "TP. Hồ Chí Minh",
                Country = "Vietnam",
                IsDefault = true,
                UserId = users[3].Id
            },
            new Address
            {
                AddressLine = "456 Đường Lê Lợi",
                Ward = "Phường Bến Nghé",
                District = "Quận 1",
                City = "TP. Hồ Chí Minh",
                Country = "Vietnam",
                IsDefault = true,
                UserId = users[4].Id
            },
            new Address
            {
                AddressLine = "789 Đường Hai Bà Trưng",
                Ward = "Phường Bếh Nghé",
                District = "Quận 1",
                City = "TP. Hồ Chí Minh",
                Country = "Vietnam",
                IsDefault = true,
                UserId = users[5].Id
            }
        };
        context.Addresses.AddRange(addresses);

        // ==================== TOURIST SPOTS ====================
        var touristSpots = new List<TouristSpot>
        {
            new TouristSpot
            {
                Name = "Vịnh Hạ Long",
                Description = "Vịnh Hạ Long là một vịnh nhỏ ở vùng đông bắc Việt Nam, bao gồm vùng biển đảo thuộc thành phố Hạ Long, tỉnh Quảng Ninh. Với hơn 1.600 đảo và bãi đá có hình thù kỳ thú, vịnh được UNESCO công nhận là Di sản Thiên nhiên Thế giới.",
                Region = "North",
                Type = "Beach",
                Address = "Thành phố Hạ Long, Quảng Ninh",
                City = "Hạ Long",
                Latitude = 20.9101,
                Longitude = 107.1839,
                Images = "[\"https://images.unsplash.com/photo-1528127269322-539801943592?w=800\",\"https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800\"]",
                TicketPrice = 250000,
                BestTime = "Tháng 3 - Tháng 5, Tháng 9 - Tháng 11",
                Rating = 4.7,
                ReviewCount = 1250,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new TouristSpot
            {
                Name = "Phong Nha - Kẻ Bàng",
                Description = "Vườn quốc gia Phong Nha - Kẻ Bàng nổi tiếng với hệ thống hang động đẹp nhất Đông Nam Á, bao gồm hang Sơn Đoòng - hang động lớn nhất thế giới.",
                Region = "Central",
                Type = "Historical",
                Address = "xã Sơn Trạch, huyện Bố Trạch, Quảng Bình",
                City = "Đồng Hới",
                Latitude = 17.5997,
                Longitude = 106.2917,
                Images = "[\"https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800\"]",
                TicketPrice = 150000,
                BestTime = "Tháng 4 - Tháng 8",
                Rating = 4.8,
                ReviewCount = 890,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new TouristSpot
            {
                Name = "Đà Lạt",
                Description = "Thành phố Đà Lạt nằm trên cao nguyên Lâm Viên, được biết đến với khí hậu mát mẻ quanh năm, cảnh quan thiên nhiên tuyệt đẹp và các khu du lịch nổi tiếng.",
                Region = "Central",
                Type = "Mountain",
                Address = "Thành phố Đà Lạt, Lâm Đồng",
                City = "Đà Lạt",
                Latitude = 11.9404,
                Longitude = 108.4583,
                Images = "[\"https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800\"]",
                TicketPrice = 0,
                BestTime = "Tháng 10 - Tháng 3",
                Rating = 4.6,
                ReviewCount = 2100,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new TouristSpot
            {
                Name = "Huế",
                Description = "Cố đô Huế là di sản văn hóa thế giới với các công trình kiến trúc độc đáo thời Nguyễn, đặc biệt là Kinh thành Huế và các lăng tẩm.",
                Region = "Central",
                Type = "Historical",
                Address = "Thành phố Huế, Thừa Thiên Huế",
                City = "Huế",
                Latitude = 16.0544,
                Longitude = 107.7222,
                Images = "[\"https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800\"]",
                TicketPrice = 200000,
                BestTime = "Tháng 3 - Tháng 5",
                Rating = 4.5,
                ReviewCount = 1800,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new TouristSpot
            {
                Name = "Phú Quốc",
                Description = "Đảo Phú Quốc là hòn đảo lớn nhất Việt Nam, nổi tiếng với bãi biển đẹp, nước trong xanh và các khu nghỉ dưỡng cao cấp.",
                Region = "South",
                Type = "Beach",
                Address = "Thành phố Phú Quốc, Kiên Giang",
                City = "Phú Quốc",
                Latitude = 10.2298,
                Longitude = 103.9667,
                Images = "[\"https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800\"]",
                TicketPrice = 0,
                BestTime = "Tháng 11 - Tháng 3",
                Rating = 4.4,
                ReviewCount = 1650,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new TouristSpot
            {
                Name = "Sapa",
                Description = "Thị trấn Sapa nằm ở độ cao 1.500m so với mực nước biển, nổi tiếng với cảnh quan núi rừng hùng vĩ, ruộng bậc thang và văn hóa các dân tộc thiểu số.",
                Region = "North",
                Type = "Mountain",
                Address = "Huyện Sapa, Lào Cai",
                City = "Sapa",
                Latitude = 22.3362,
                Longitude = 103.8443,
                Images = "[\"https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800\"]",
                TicketPrice = 0,
                BestTime = "Tháng 9 - Tháng 11, Tháng 3 - Tháng 5",
                Rating = 4.5,
                ReviewCount = 1450,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new TouristSpot
            {
                Name = "Nha Trang",
                Description = "Thành phố Nha Trang được biết đến là một trong những bãi biển đẹp nhất Việt Nam với bờ biển dài 6km và các đảo xung quanh.",
                Region = "South",
                Type = "Beach",
                Address = "Thành phố Nha Trang, Khánh Hòa",
                City = "Nha Trang",
                Latitude = 12.2388,
                Longitude = 109.1967,
                Images = "[\"https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800\"]",
                TicketPrice = 0,
                BestTime = "Tháng 4 - Tháng 9",
                Rating = 4.3,
                ReviewCount = 2200,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new TouristSpot
            {
                Name = "Hội An",
                Description = "Phố cổ Hội An là đô thị cổ có kiến trúc độc đáo, mang đậm phong cách Đông Nam Á kết hợp với ảnh hưởng Trung Quốc và Nhật Bản.",
                Region = "Central",
                Type = "Historical",
                Address = "Thành phố Hội An, Quảng Nam",
                City = "Hội An",
                Latitude = 15.8794,
                Longitude = 108.3350,
                Images = "[\"https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800\"]",
                TicketPrice = 150000,
                BestTime = "Tháng 2 - Tháng 4",
                Rating = 4.7,
                ReviewCount = 3100,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            }
        };
        context.TouristSpots.AddRange(touristSpots);
        await context.SaveChangesAsync();

        // ==================== HOTELS ====================
        var hotels = new List<Hotel>
        {
            new Hotel
            {
                Name = "InterContinental Hanoi Westlake",
                Description = "Khách sạn 5 sao sang trọng với view hồ Tây tuyệt đẹp, located on the peaceful banks of West Lake.",
                Address = "1A Nghi Tam, Tay Ho",
                City = "Hà Nội",
                StarRating = 5,
                ContactName = "Mr. John Smith",
                ContactPhone = "024 3670 3333",
                ContactEmail = "reservation@ic-hanoi.com",
                Images = "[\"https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800\"]",
                MinPrice = 3500000,
                MaxPrice = 15000000,
                Amenities = "[\"Wifi\",\"Pool\",\"Spa\",\"Gym\",\"Restaurant\",\"Bar\",\"Room Service\"]",
                CancellationPolicy = "Miễn phí hủy trước 24 giờ",
                CheckInTime = "14:00",
                CheckOutTime = "12:00",
                Rating = 4.8,
                ReviewCount = 450,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Hotel
            {
                Name = "The Deck Saigon",
                Description = "Boutique hotel sang trọng bên sông Sài Gòn với tầm nhìn panorama tuyệt đẹp.",
                Address = "38 Nguyen U Di, District 2",
                City = "TP. Hồ Chí Minh",
                StarRating = 4,
                ContactName = "Ms. Sarah Lee",
                ContactPhone = "028 3622 4333",
                ContactEmail = "book@thedeck.vn",
                Images = "[\"https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800\"]",
                MinPrice = 1800000,
                MaxPrice = 5000000,
                Amenities = "[\"Wifi\",\"Pool\",\"Restaurant\",\"Bar\",\"Spa\"]",
                CancellationPolicy = "Miễn phí hủy trước 48 giờ",
                CheckInTime = "15:00",
                CheckOutTime = "11:00",
                Rating = 4.6,
                ReviewCount = 320,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Hotel
            {
                Name = "Vinpearl Resort & Spa Nha Trang",
                Description = "Khu nghỉ dưỡng 5 sao sang trọng với bãi biển riêng, casino và công viên giải trí.",
                Address = "Đường Trần Phú, Vinh Nguyen",
                City = "Nha Trang",
                StarRating = 5,
                ContactName = "Mr. David Tran",
                ContactPhone = "025 8 222 9999",
                ContactEmail = "reservation@vinpearl.com",
                Images = "[\"https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800\"]",
                MinPrice = 4500000,
                MaxPrice = 25000000,
                Amenities = "[\"Wifi\",\"Pool\",\"Beach\",\"Spa\",\"Gym\",\"Casino\",\"Theme Park\"]",
                CancellationPolicy = "Miễn phí hủy trước 72 giờ",
                CheckInTime = "14:00",
                CheckOutTime = "12:00",
                Rating = 4.7,
                ReviewCount = 680,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Hotel
            {
                Name = "Ana Mandara Resort Dalat",
                Description = "Khu nghỉ dưỡng 4 sao theo phong cách Pháp cổ điển giữa thành phố Đà Lạt mộng mơ.",
                Address = "32 Truong Cong Dinh, Ward 1",
                City = "Đà Lạt",
                StarRating = 4,
                ContactName = "Ms. Hoa Nguyen",
                ContactPhone = "0263 382 2222",
                ContactEmail = "stay@anamandara.vn",
                Images = "[\"https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800\"]",
                MinPrice = 2200000,
                MaxPrice = 8000000,
                Amenities = "[\"Wifi\",\"Spa\",\"Restaurant\",\"Bar\",\"Garden\"]",
                CancellationPolicy = "Miễn phí hủy trước 48 giờ",
                CheckInTime = "14:00",
                CheckOutTime = "12:00",
                Rating = 4.5,
                ReviewCount = 210,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Hotel
            {
                Name = "Golden Bay Hotel Ha Long",
                Description = "Khách sạn 5 sao với view vịnh Hạ Long tuyệt đẹp, tiêu chuẩn quốc tế.",
                Address = "1st Floor, 1 Ha Long Road",
                City = "Hạ Long",
                StarRating = 5,
                ContactName = "Mr. Hung Vu",
                ContactPhone = "0203 384 8888",
                ContactEmail = "reservation@goldenbayhotel.vn",
                Images = "[\"https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800\"]",
                MinPrice = 2800000,
                MaxPrice = 12000000,
                Amenities = "[\"Wifi\",\"Pool\",\"Spa\",\"Gym\",\"Restaurant\",\"Bar\",\"Boat Tour\"]",
                CancellationPolicy = "Miễn phí hủy trước 24 giờ",
                CheckInTime = "14:00",
                CheckOutTime = "12:00",
                Rating = 4.6,
                ReviewCount = 380,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Hotel
            {
                Name = "Hotel Royal Hoi An - MGallery",
                Description = "Khách sạn sang trọng bên bờ sông Hội An, kết hợp kiến trúc Pháp và Đông Nam Á.",
                Address = "39 Dao Duy Tu, Hoi An",
                City = "Hội An",
                StarRating = 5,
                ContactName = "Ms. Linh Pham",
                ContactPhone = "0235 392 7070",
                ContactEmail = "hotelroyalhoi.an@accor.com",
                Images = "[\"https://images.unsplash.com/photo-1566073771259-6a8506099945?w=800\"]",
                MinPrice = 4000000,
                MaxPrice = 18000000,
                Amenities = "[\"Wifi\",\"Pool\",\"Spa\",\"Gym\",\"Restaurant\",\"Bar\",\"Bicycle\"]",
                CancellationPolicy = "Miễn phí hủy trước 48 giờ",
                CheckInTime = "15:00",
                CheckOutTime = "11:00",
                Rating = 4.8,
                ReviewCount = 290,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            }
        };
        context.Hotels.AddRange(hotels);
        await context.SaveChangesAsync();

        // ==================== HOTEL ROOMS ====================
        var hotelRooms = new List<HotelRoom>();
        foreach (var hotel in hotels)
        {
            hotelRooms.Add(new HotelRoom
            {
                RoomType = "Standard",
                Description = "Phòng Standard với đầy đủ tiện nghi, view thành phố",
                MaxOccupancy = 2,
                PricePerNight = hotel.MinPrice ?? 1000000,
                BedType = "Double",
                RoomAmenities = "[\"Wifi\",\"TV\",\"Air Conditioning\",\"Minibar\",\"Safe\"]",
                Images = "[\"https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800\"]",
                TotalRooms = 20,
                AvailableRooms = 15,
                HotelId = hotel.Id,
                IsActive = true,
                IsDeleted = false
            });

            hotelRooms.Add(new HotelRoom
            {
                RoomType = "Deluxe",
                Description = "Phòng Deluxe rộng rãi hơn với view đẹp",
                MaxOccupancy = 3,
                PricePerNight = (hotel.MinPrice ?? 1000000) * 1.5m,
                BedType = "Twin/Double",
                RoomAmenities = "[\"Wifi\",\"TV\",\"Air Conditioning\",\"Minibar\",\"Safe\",\"Bathtub\"]",
                Images = "[\"https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800\"]",
                TotalRooms = 15,
                AvailableRooms = 10,
                HotelId = hotel.Id,
                IsActive = true,
                IsDeleted = false
            });

            hotelRooms.Add(new HotelRoom
            {
                RoomType = "Suite",
                Description = "Phòng Suite cao cấp với không gian riêng biệt và tiện nghi sang trọng",
                MaxOccupancy = 4,
                PricePerNight = (hotel.MinPrice ?? 1000000) * 2.5m,
                BedType = "King",
                RoomAmenities = "[\"Wifi\",\"TV\",\"Air Conditioning\",\"Minibar\",\"Safe\",\"Bathtub\",\"Living Room\",\"Butler Service\"]",
                Images = "[\"https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=800\"]",
                TotalRooms = 5,
                AvailableRooms = 3,
                HotelId = hotel.Id,
                IsActive = true,
                IsDeleted = false
            });
        }
        context.HotelRooms.AddRange(hotelRooms);

        // ==================== RESTAURANTS ====================
        var restaurants = new List<Restaurant>
        {
            new Restaurant
            {
                Name = "Nha Hang Ngon",
                Description = "Nhà hàng phục vụ các món ăn Việt Nam truyền thống với không gian sang trọng",
                Address = "18 Phan Boi Chau, District 1",
                City = "TP. Hồ Chí Minh",
                CuisineType = "Vietnamese",
                PriceLevel = "MidRange",
                Style = "Restaurant",
                OpeningTime = "07:00",
                ClosingTime = "22:00",
                ContactPhone = "028 3822 2115",
                Images = "[\"https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=800\"]",
                Menu = "[{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Phở Bò\",\"Description\":\"Phở bò tái nạm đặc trưng Việt Nam\",\"Price\":65000,\"ImageUrl\":null,\"Category\":\"Main\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":false},{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Bún Chả\",\"Description\":\"Bún chả Hà Nội truyền thống\",\"Price\":55000,\"ImageUrl\":null,\"Category\":\"Main\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":false},{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Cơm Tấm\",\"Description\":\"Cơm tấm sườn nướng\",\"Price\":45000,\"ImageUrl\":null,\"Category\":\"Main\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":false}]",
                HasReservation = true,
                Rating = 4.4,
                ReviewCount = 520,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Restaurant
            {
                Name = "Den Long",
                Description = "Nhà hàng ẩm thực Việt - Nhật với không gian ấm cúng",
                Address = "129 Nguyen Thai Hoc, Hue City",
                City = "Huế",
                CuisineType = "Vietnamese",
                PriceLevel = "HighEnd",
                Style = "Restaurant",
                OpeningTime = "10:00",
                ClosingTime = "23:00",
                ContactPhone = "0234 383 4242",
                Images = "[\"https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=800\"]",
                Menu = "[{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Bún Bò Huế\",\"Description\":\"Bún bò Huế đặc trưng\",\"Price\":55000,\"ImageUrl\":null,\"Category\":\"Main\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":true},{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Cơm Hến\",\"Description\":\"Cơm hến Huế\",\"Price\":35000,\"ImageUrl\":null,\"Category\":\"Main\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":false}]",
                HasReservation = true,
                Rating = 4.6,
                ReviewCount = 180,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Restaurant
            {
                Name = "La Vache",
                Description = "Nhà hàng Pháp nổi tiếng với món bò beefsteak chất lượng cao",
                Address = "1st Floor, 79 Le Xuan Dang, District 1",
                City = "TP. Hồ Chí Minh",
                CuisineType = "French",
                PriceLevel = "HighEnd",
                Style = "Restaurant",
                OpeningTime = "11:00",
                ClosingTime = "23:00",
                ContactPhone = "028 5412 4990",
                Images = "[\"https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=800\"]",
                Menu = "[{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Beefsteak\",\"Description\":\"Bò beefsteak Mỹ chất lượng cao\",\"Price\":350000,\"ImageUrl\":null,\"Category\":\"Main\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":false},{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"French Onion Soup\",\"Description\":\"Soup hành tây kiểu Pháp\",\"Price\":89000,\"ImageUrl\":null,\"Category\":\"Appetizer\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":false},{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Tiramisu\",\"Description\":\"Tiramisu Ý truyền thống\",\"Price\":75000,\"ImageUrl\":null,\"Category\":\"Dessert\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":false}]",
                HasReservation = true,
                Rating = 4.7,
                ReviewCount = 350,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Restaurant
            {
                Name = "Sorae Sushi",
                Description = "Nhà hàng Nhật Bản chính hãng với đầu bếp từ Tokyo",
                Address = "Lotte Tower, 54 Lieu Giai, Ba Dinh",
                City = "Hà Nội",
                CuisineType = "Japanese",
                PriceLevel = "Luxury",
                Style = "Restaurant",
                OpeningTime = "11:00",
                ClosingTime = "22:00",
                ContactPhone = "024 3333 9888",
                Images = "[\"https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=800\"]",
                Menu = "[{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Sushi Set\",\"Description\":\"Sushi tươi ngon Nhật Bản\",\"Price\":450000,\"ImageUrl\":null,\"Category\":\"Main\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":false},{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Sashimi\",\"Description\":\"Sashimi cá hồi tươi\",\"Price\":380000,\"ImageUrl\":null,\"Category\":\"Main\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":false},{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Ramen\",\"Description\":\"Ramen truyền thống\",\"Price\":120000,\"ImageUrl\":null,\"Category\":\"Main\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":false}]",
                HasReservation = true,
                Rating = 4.8,
                ReviewCount = 420,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Restaurant
            {
                Name = "The Deck seafood",
                Description = "Nhà hàng hải sản tươi sống bên sông Sài Gòn",
                Address = "38 Nguyen U Di, District 2",
                City = "TP. Hồ Chí Minh",
                CuisineType = "Seafood",
                PriceLevel = "HighEnd",
                Style = "Restaurant",
                OpeningTime = "10:00",
                ClosingTime = "23:00",
                ContactPhone = "028 3622 4333",
                Images = "[\"https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=800\"]",
                Menu = "[{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Tôm Hùm\",\"Description\":\"Tôm hùm tươi sống\",\"Price\":650000,\"ImageUrl\":null,\"Category\":\"Main\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":false},{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Cua Rang\",\"Description\":\"Cua rang muối\",\"Price\":450000,\"ImageUrl\":null,\"Category\":\"Main\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":false},{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Lẩu Hải Sản\",\"Description\":\"Lẩu hải sản tươi ngon\",\"Price\":380000,\"ImageUrl\":null,\"Category\":\"Main\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":true}]",
                HasReservation = true,
                Rating = 4.5,
                ReviewCount = 290,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Restaurant
            {
                Name = "Café du Lac",
                Description = "Quán cà phê lãng mạn bên hồ Tây với view đẹp",
                Address = "2 Nguyen Thai Hoc, Tay Ho",
                City = "Hà Nội",
                CuisineType = "Cafe",
                PriceLevel = "MidRange",
                Style = "Cafe",
                OpeningTime = "07:00",
                ClosingTime = "22:00",
                ContactPhone = "024 3715 2288",
                Images = "[\"https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=800\"]",
                Menu = "[{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Cà phê sữa đá\",\"Description\":\"Cà phê sữa đá truyền thống Việt Nam\",\"Price\":29000,\"ImageUrl\":null,\"Category\":\"Drink\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":false},{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Cà phê đen\",\"Description\":\"Cà phê đen không đường\",\"Price\":25000,\"ImageUrl\":null,\"Category\":\"Drink\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":false},{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Trà sen\",\"Description\":\"Trà sen thơm dịu\",\"Price\":35000,\"ImageUrl\":null,\"Category\":\"Drink\",\"IsAvailable\":true,\"IsVegetarian\":true,\"IsSpicy\":false}]",
                HasReservation = false,
                Rating = 4.3,
                ReviewCount = 650,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Restaurant
            {
                Name = "Quán Ốc Oải",
                Description = "Quán ốc ngon nổi tiếng Sài Gòn",
                Address = "312 Nguyen Trai, District 1",
                City = "TP. Hồ Chí Minh",
                CuisineType = "Seafood",
                PriceLevel = "Budget",
                Style = "Restaurant",
                OpeningTime = "15:00",
                ClosingTime = "23:00",
                ContactPhone = "028 3853 6565",
                Images = "[\"https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=800\"]",
                Menu = "[{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Ốc len xào dừa\",\"Description\":\"Ốc len xào dừa béo ngậy\",\"Price\":85000,\"ImageUrl\":null,\"Category\":\"Main\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":false},{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Ốc nướng tiêu\",\"Description\":\"Ốc nướng tiêu xanh\",\"Price\":65000,\"ImageUrl\":null,\"Category\":\"Main\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":true}]",
                HasReservation = false,
                Rating = 4.2,
                ReviewCount = 780,
                IsFeatured = false,
                IsActive = true,
                IsDeleted = false
            },
            new Restaurant
            {
                Name = "Highland Coffee",
                Description = "Chuỗi cà phê Việt Nam nổi tiếng",
                Address = "Multiple locations",
                City = "TP. Hồ Chí Minh",
                CuisineType = "Cafe",
                PriceLevel = "Budget",
                Style = "Cafe",
                OpeningTime = "07:00",
                ClosingTime = "22:00",
                ContactPhone = "1900 6788",
                Images = "[\"https://images.unsplash.com/photo-1554118811-1e0d58224f24?w=800\"]",
                Menu = "[{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Cà phê\",\"Description\":\"Cà phê Highland\",\"Price\":35000,\"ImageUrl\":null,\"Category\":\"Drink\",\"IsAvailable\":true,\"IsVegetarian\":false,\"IsSpicy\":false},{\"MenuItemId\":\"" + Guid.NewGuid() + "\",\"Name\":\"Trà trái cây\",\"Description\":\"Trà trái cây tươi\",\"Price\":45000,\"ImageUrl\":null,\"Category\":\"Drink\",\"IsAvailable\":true,\"IsVegetarian\":true,\"IsSpicy\":false}]",
                HasReservation = false,
                Rating = 4.0,
                ReviewCount = 2500,
                IsFeatured = false,
                IsActive = true,
                IsDeleted = false
            }
        };
        context.Restaurants.AddRange(restaurants);
        await context.SaveChangesAsync();

        // ==================== RESORTS ====================
        var resorts = new List<Resort>
        {
            new Resort
            {
                Name = "Six Senses Ninh Van Bay",
                Description = "Khu nghỉ dưỡng 5 sao sang trọng bên vịnh riêng, với biệt thự trên bờ và trên mặt nước",
                Address = "Ninh Van Bay, Ninh Hoa",
                City = "Nha Trang",
                LocationType = "Beach",
                StarRating = 5,
                Images = "[\"https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800\"]",
                MinPrice = 15000000,
                MaxPrice = 50000000,
                Amenities = "[\"Private Beach\",\"Pool\",\"Spa\",\"Diving Center\",\"Restaurant\",\"Bar\",\"Kids Club\"]",
                Activities = null,
                Packages = null,
                Rating = 4.9,
                ReviewCount = 180,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Resort
            {
                Name = "Four Seasons Resort The Nam Hai",
                Description = "Khu nghỉ dưỡng sang trọng bên bờ biển Cửa Đại với view biển tuyệt đẹp",
                Address = "Block 1, Cua Dai Beach",
                City = "Hội An",
                LocationType = "Beach",
                StarRating = 5,
                Images = "[\"https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800\"]",
                MinPrice = 12000000,
                MaxPrice = 45000000,
                Amenities = "[\"Private Beach\",\"Pool\",\"Spa\",\"Gym\",\"Restaurant\",\"Bar\",\"Cooking School\"]",
                Activities = null,
                Packages = null,
                Rating = 4.8,
                ReviewCount = 250,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Resort
            {
                Name = "Azerai Ke Ga Bay",
                Description = "Khu nghỉ dưỡng yên tĩnh với bãi biển hoang sơ và dịch vụ cao cấp",
                Address = "Ke Ga Bay, Ho Chi Minh",
                City = "Phan Thiết",
                LocationType = "Beach",
                StarRating = 5,
                Images = "[\"https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800\"]",
                MinPrice = 8000000,
                MaxPrice = 25000000,
                Amenities = "[\"Private Beach\",\"Pool\",\"Spa\",\"Restaurant\",\"Bar\",\"Water Sports\"]",
                Activities = null,
                Packages = null,
                Rating = 4.7,
                ReviewCount = 120,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Resort
            {
                Name = "Emeralda Resort Ninh Binh",
                Description = "Khu nghỉ dưỡng giữa thiên nhiên với view núi Non Nước tuyệt đẹp",
                Address = "Van Long Wetland Reserve",
                City = "Ninh Bình",
                LocationType = "Eco",
                StarRating = 4,
                Images = "[\"https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800\"]",
                MinPrice = 3500000,
                MaxPrice = 12000000,
                Amenities = "[\"Pool\",\"Spa\",\"Restaurant\",\"Bar\",\"Bicycle Rental\"]",
                Activities = null,
                Packages = null,
                Rating = 4.5,
                ReviewCount = 160,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Resort
            {
                Name = "Victoria Sapa Resort & Spa",
                Description = "Khu nghỉ dưỡng 4 sao theo phong cách Pháp giữa thị trấn Sapa mù mịt mây",
                Address = "Phan Si Pang Street",
                City = "Sapa",
                LocationType = "Mountain",
                StarRating = 4,
                Images = "[\"https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=800\"]",
                MinPrice = 2500000,
                MaxPrice = 8000000,
                Amenities = "[\"Spa\",\"Restaurant\",\"Bar\",\"Heated Pool\",\"Gym\"]",
                Activities = null,
                Packages = null,
                Rating = 4.4,
                ReviewCount = 210,
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            }
        };
        context.Resorts.AddRange(resorts);
        await context.SaveChangesAsync();

        // ==================== RESORT ROOMS ====================
        var resortRooms = new List<ResortRoom>();
        foreach (var resort in resorts)
        {
            resortRooms.Add(new ResortRoom
            {
                RoomType = "Bungalow",
                Description = "Biệt thự bungalow với view vườn hoặc núi",
                MaxOccupancy = 2,
                PricePerNight = resort.MinPrice ?? 2000000,
                BedType = "Double",
                RoomAmenities = "[\"Wifi\",\"AC\",\"Minibar\",\"Balcony\",\"Hot Tub\"]",
                Images = "[\"https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800\"]",
                TotalRooms = 10,
                AvailableRooms = 7,
                ResortId = resort.Id,
                IsActive = true,
                IsDeleted = false
            });

            resortRooms.Add(new ResortRoom
            {
                RoomType = "Villa",
                Description = "Biệt thự riêng biệt với hồ bơi và view tuyệt đẹp",
                MaxOccupancy = 4,
                PricePerNight = (resort.MinPrice ?? 2000000) * 2.5m,
                BedType = "King",
                RoomAmenities = "[\"Private Pool\",\"Wifi\",\"AC\",\"Minibar\",\"Butler Service\",\"Large Balcony\"]",
                Images = "[\"https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800\"]",
                TotalRooms = 5,
                AvailableRooms = 3,
                ResortId = resort.Id,
                IsActive = true,
                IsDeleted = false
            });

            resortRooms.Add(new ResortRoom
            {
                RoomType = "Suite",
                Description = "Suite cao cấp với không gian rộng rãi và dịch vụ đặc biệt",
                MaxOccupancy = 3,
                PricePerNight = (resort.MinPrice ?? 2000000) * 1.8m,
                BedType = "Twin/Double",
                RoomAmenities = "[\"Wifi\",\"AC\",\"Minibar\",\"Spa Access\",\"Living Area\"]",
                Images = "[\"https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=800\"]",
                TotalRooms = 8,
                AvailableRooms = 5,
                ResortId = resort.Id,
                IsActive = true,
                IsDeleted = false
            });
        }
        context.ResortRooms.AddRange(resortRooms);

        // ==================== TRANSPORTS ====================
        var transports = new List<Transport>
        {
            new Transport
            {
                Type = "Flight",
                Provider = "Vietnam Airlines",
                FromCity = "TP. Hồ Chí Minh",
                ToCity = "Hà Nội",
                Route = "SGN - HAN",
                DepartureTime = TimeSpan.FromHours(7),
                ArrivalTime = TimeSpan.FromHours(9.5),
                DurationMinutes = 150,
                Price = 2500000,
                AvailableSeats = 180,
                Amenities = "[\"WiFi\",\"Meal\",\"Entertainment\",\"Luggage\"]",
                Images = "[\"https://images.unsplash.com/photo-1436491865332-7a61a109cc05?w=800\"]",
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Transport
            {
                Type = "Flight",
                Provider = "VietJet Air",
                FromCity = "TP. Hồ Chí Minh",
                ToCity = "Đà Lạt",
                Route = "SGN - DAD",
                DepartureTime = TimeSpan.FromHours(6),
                ArrivalTime = TimeSpan.FromHours(7),
                DurationMinutes = 60,
                Price = 890000,
                AvailableSeats = 180,
                Amenities = "[\"Meal\",\"Entertainment\"]",
                Images = "[\"https://images.unsplash.com/photo-1436491865332-7a61a109cc05?w=800\"]",
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Transport
            {
                Type = "Flight",
                Provider = " Bamboo Airways",
                FromCity = "Hà Nội",
                ToCity = "Phú Quốc",
                Route = "HAN - PQC",
                DepartureTime = TimeSpan.FromHours(8),
                ArrivalTime = TimeSpan.FromHours(11),
                DurationMinutes = 180,
                Price = 3200000,
                AvailableSeats = 160,
                Amenities = "[\"WiFi\",\"Meal\",\"Entertainment\",\"Luggage\"]",
                Images = "[\"https://images.unsplash.com/photo-1436491865332-7a61a109cc05?w=800\"]",
                IsFeatured = false,
                IsActive = true,
                IsDeleted = false
            },
            new Transport
            {
                Type = "Bus",
                Provider = "Futa Bus Lines",
                FromCity = "TP. Hồ Chí Minh",
                ToCity = "Đà Lạt",
                Route = "SGN - DAD",
                DepartureTime = TimeSpan.FromHours(20),
                ArrivalTime = TimeSpan.FromHours(6),
                DurationMinutes = 600,
                Price = 350000,
                AvailableSeats = 45,
                Amenities = "[\"WiFi\",\"Water\",\"Blanket\",\"AC\"]",
                Images = "[\"https://images.unsplash.com/photo-1534237710431-e2fc698436d0?w=800\"]",
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Transport
            {
                Type = "Bus",
                Provider = "Sinh Cafe",
                FromCity = "Hà Nội",
                ToCity = "Sapa",
                Route = "HAN - SAI",
                DepartureTime = TimeSpan.FromHours(6),
                ArrivalTime = TimeSpan.FromHours(12),
                DurationMinutes = 360,
                Price = 250000,
                AvailableSeats = 30,
                Amenities = "[\"Water\",\"Snack\"]",
                Images = "[\"https://images.unsplash.com/photo-1534237710431-e2fc698436d0?w=800\"]",
                IsFeatured = false,
                IsActive = true,
                IsDeleted = false
            },
            new Transport
            {
                Type = "Train",
                Provider = "Vietnam Railway",
                FromCity = "Hà Nội",
                ToCity = "Huế",
                Route = "HAN - HUE",
                DepartureTime = TimeSpan.FromHours(19),
                ArrivalTime = TimeSpan.FromHours(8),
                DurationMinutes = 780,
                Price = 800000,
                AvailableSeats = 40,
                Amenities = "[\"AC\",\"Bed\",\"Meal\",\"WiFi\"]",
                Images = "[\"https://images.unsplash.com/photo-1474487548417-781cb71495f3?w=800\"]",
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            },
            new Transport
            {
                Type = "Train",
                Provider = "Vietnam Railway",
                FromCity = "TP. Hồ Chí Minh",
                ToCity = "Nha Trang",
                Route = "SGN - NHA",
                DepartureTime = TimeSpan.FromHours(22),
                ArrivalTime = TimeSpan.FromHours(7),
                DurationMinutes = 540,
                Price = 600000,
                AvailableSeats = 40,
                Amenities = "[\"AC\",\"Bed\",\"Meal\"]",
                Images = "[\"https://images.unsplash.com/photo-1474487548417-781cb71495f3?w=800\"]",
                IsFeatured = false,
                IsActive = true,
                IsDeleted = false
            },
            new Transport
            {
                Type = "Limousine",
                Provider = "Mai Linh Limousine",
                FromCity = "TP. Hồ Chí Minh",
                ToCity = "Vũng Tàu",
                Route = "SGN - VTA",
                DepartureTime = TimeSpan.FromHours(8),
                ArrivalTime = TimeSpan.FromHours(10),
                DurationMinutes = 120,
                Price = 450000,
                AvailableSeats = 9,
                Amenities = "[\"WiFi\",\"Water\",\"AC\",\"Reclining Seats\"]",
                Images = "[\"https://images.unsplash.com/photo-1549317661-bd32c8ce0db2?w=800\"]",
                IsFeatured = true,
                IsActive = true,
                IsDeleted = false
            }
        };
        context.Transports.AddRange(transports);
        await context.SaveChangesAsync();

        // ==================== TOUR PACKAGES ====================
        var tourPackages = new List<TourPackage>
        {
            new TourPackage
            {
                Name = "Tour Hạ Long 3 Ngày 2 Đêm",
                Description = "Khám phá vịnh Hạ Long tuyệt đẹp với chuyến du ngoạn trên tàu VIP, tham quan hang động và đảo hoang sơ.",
                Destination = "Hạ Long - Quảng Ninh",
                DurationDays = 3,
                Price = 4500000,
                DiscountPrice = 3990000,
                Images = "[\"https://images.unsplash.com/photo-1528127269322-539801943592?w=800\"]",
                Gallery = "[\"https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800\"]",
                Includes = "[\"Khách sạn 4 sao\",\"Tàu tham quan vịnh\",\"Vé tham quan\",\"Hướng dẫn viên\",\"Bữa ăn\"]",
                Excludes = "[\"Đồ uống cá nhân\",\"Chi phí cá nhân\",\"Tip cho hướng dẫn\"]",
                AvailableSlots = 20,
                DepartureDates = "[\"2025-03-15\",\"2025-03-22\",\"2025-03-29\"]",
                Rating = 4.6,
                ReviewCount = 180,
                IsFeatured = true,
                IsNewArrival = false,
                IsHotDeal = true,
                IsActive = true,
                IsDeleted = false
            },
            new TourPackage
            {
                Name = "Tour Đà Lạt 4 Ngày 3 Đêm",
                Description = "Khám phá thành phố mù mịt với các địa điểm nổi tiếng: Thung lũng Tình Yêu, Hồ Xuân Hương, Crazy House...",
                Destination = "Đà Lạt - Lâm Đồng",
                DurationDays = 4,
                Price = 3800000,
                DiscountPrice = 3490000,
                Images = "[\"https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800\"]",
                Includes = "[\"Khách sạn 3 sao\",\"Xe di chuyển\",\"Vé tham quan\",\"Hướng dẫn viên\",\"Bữa ăn\"]",
                Excludes = "[\"Đồ uống\",\"Chi phí cá nhân\"]",
                AvailableSlots = 25,
                DepartureDates = "[\"2025-03-20\",\"2025-04-01\",\"2025-04-10\"]",
                Rating = 4.5,
                ReviewCount = 250,
                IsFeatured = true,
                IsNewArrival = true,
                IsHotDeal = false,
                IsActive = true,
                IsDeleted = false
            },
            new TourPackage
            {
                Name = "Tour Phú Quốc 5 Ngày 4 Đêm",
                Description = "Trải nghiệm thiên đường biển đảo Phú Quốc với bãi biển hoang sơ, rừng nguyên sinh và hải sản tươi ngon.",
                Destination = "Phú Quốc - Kiên Giang",
                DurationDays = 5,
                Price = 7200000,
                DiscountPrice = 6500000,
                Images = "[\"https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800\"]",
                Includes = "[\"Resort 4 sao\",\"Xe đưa đón\",\"Tour tham quan\",\"Hướng dẫn viên\",\"Bữa ăn\"]",
                Excludes = "[\"Vé máy bay\",\"Đồ uống\",\"Chi phí cá nhân\"]",
                AvailableSlots = 15,
                DepartureDates = "[\"2025-03-25\",\"2025-04-05\",\"2025-04-15\"]",
                Rating = 4.7,
                ReviewCount = 120,
                IsFeatured = true,
                IsNewArrival = false,
                IsHotDeal = true,
                IsActive = true,
                IsDeleted = false
            },
            new TourPackage
            {
                Name = "Tour Hội An - Huế 3 Ngày 2 Đêm",
                Description = "Khám phá di sản văn hóa thế giới với phố cổ Hội An và cố đô Huế.",
                Destination = "Hội An - Huế",
                DurationDays = 3,
                Price = 4200000,
                Images = "[\"https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800\"]",
                Includes = "[\"Khách sạn 3 sao\",\"Xe di chuyển\",\"Vé tham quan\",\"Hướng dẫn viên\",\"Bữa ăn\"]",
                Excludes = "[\"Đồ uống\",\"Chi phí cá nhân\"]",
                AvailableSlots = 20,
                DepartureDates = "[\"2025-03-18\",\"2025-04-02\",\"2025-04-12\"]",
                Rating = 4.4,
                ReviewCount = 320,
                IsFeatured = true,
                IsNewArrival = false,
                IsHotDeal = false,
                IsActive = true,
                IsDeleted = false
            },
            new TourPackage
            {
                Name = "Tour Sapa 2 Ngày 1 Đêm",
                Description = "Khám phá thị trấn Sapa mù mịt mây với ruộng bậc thang và văn hóa các dân tộc thiểu số.",
                Destination = "Sapa - Lào Cai",
                DurationDays = 2,
                Price = 2500000,
                DiscountPrice = 2200000,
                Images = "[\"https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800\"]",
                Includes = "[\"Khách sạn 3 sao\",\"Xe bus\",\"Hướng dẫn viên\",\"Bữa ăn\"]",
                Excludes = "[\"Đồ uống\",\"Chi phí cá nhân\",\"Tip\"]",
                AvailableSlots = 30,
                DepartureDates = "[\"2025-03-16\",\"2025-03-23\",\"2025-03-30\"]",
                Rating = 4.3,
                ReviewCount = 450,
                IsFeatured = false,
                IsNewArrival = false,
                IsHotDeal = true,
                IsActive = true,
                IsDeleted = false
            },
            new TourPackage
            {
                Name = "Tour Nha Trang 4 Ngày 3 Đêm",
                Description = "Tận hưởng bãi biển Nha Trang xanh trong với các hoạt động biển đa dạng.",
                Destination = "Nha Trang - Khánh Hòa",
                DurationDays = 4,
                Price = 5500000,
                Images = "[\"https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800\"]",
                Includes = "[\"Khách sạn 4 sao\",\"Tour tham quan\",\"Hướng dẫn viên\",\"Bữa ăn\"]",
                Excludes = "[\"Vé máy bay\",\"Đồ uống\",\"Chi phí cá nhân\"]",
                AvailableSlots = 18,
                DepartureDates = "[\"2025-03-21\",\"2025-04-03\",\"2025-04-14\"]",
                Rating = 4.5,
                ReviewCount = 280,
                IsFeatured = true,
                IsNewArrival = true,
                IsHotDeal = false,
                IsActive = true,
                IsDeleted = false
            }
        };
        context.TourPackages.AddRange(tourPackages);
        await context.SaveChangesAsync();

        // ==================== PROMOTIONS ====================
        var promotions = new List<Promotion>
        {
            new Promotion
            {
                Code = "SALE30",
                Title = "Giảm 30% Tất Cả Tour",
                Description = "Áp dụng cho tất cả tour du lịch trong nước",
                DiscountType = DiscountType.Percentage,
                DiscountValue = 30,
                MinOrderAmount = 5000000,
                MaxDiscountAmount = 2000000,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(1),
                IsActive = true,
                TargetType = PromotionTargetType.Tour,
                ShowOnHome = true,
                IsDeleted = false
            },
            new Promotion
            {
                Code = "WELCOME100",
                Title = "Up to $100 Off Your First Order",
                Description = "Get $100 off immediately for new customers",
                DiscountType = DiscountType.FixedAmount,
                DiscountValue = 100000,
                MinOrderAmount = 500000,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(3),
                IsActive = true,
                TargetType = PromotionTargetType.All,
                ShowOnHome = true,
                IsDeleted = false
            },
            new Promotion
            {
                Code = "HOTEL50",
                Title = "Giảm 50% Booking Khách Sạn",
                Description = "Giảm 50% cho khách sạn 5 sao",
                DiscountType = DiscountType.Percentage,
                DiscountValue = 50,
                MinOrderAmount = 3000000,
                MaxDiscountAmount = 3000000,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(2),
                IsActive = true,
                TargetType = PromotionTargetType.Hotel,
                ShowOnHome = false,
                IsDeleted = false
            },
            new Promotion
            {
                Code = "SUMMER25",
                Title = "Giảm 25% Mùa Hè",
                Description = "Áp dụng cho tour mùa hè 2025",
                DiscountType = DiscountType.Percentage,
                DiscountValue = 25,
                MinOrderAmount = 4000000,
                MaxDiscountAmount = 1500000,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(4),
                IsActive = true,
                TargetType = PromotionTargetType.Tour,
                ShowOnHome = true,
                IsDeleted = false
            },
            new Promotion
            {
                Code = "NEWYEAR2025",
                Title = "Khuyến Mãi Năm Mới",
                Description = "Giảm giá đặc biệt dịp năm mới",
                DiscountType = DiscountType.Percentage,
                DiscountValue = 35,
                MinOrderAmount = 6000000,
                MaxDiscountAmount = 3000000,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(2),
                IsActive = true,
                TargetType = PromotionTargetType.All,
                ShowOnHome = true,
                IsDeleted = false
            }
        };
        context.Promotions.AddRange(promotions);
        await context.SaveChangesAsync();

        // ==================== TOURS ====================
        var tours = new List<Tour>
        {
            new Tour
            {
                Name = "Du Lịch Hạ Long - Khám Phá Vịnh Kỳ Quan",
                Description = "Chuyến đi 3 ngày 2 đêm khám phá vịnh Hạ Long - một trong 7 kỳ quan thiên nhiên mới của thế giới với hơn 1.600 đảo đá vôi.",
                DurationDays = 3,
                DurationNights = 2,
                BasePrice = 4500000,
                Status = TourStatus.Active,
                ThumbnailUrl = "https://images.unsplash.com/photo-1528127269322-539801943592?w=800",
                Highlights = "[\"Tàu VIP tham quan vịnh\",\"Hang Sửng Sốt\",\"Đảo Ti Tốp\",\"Làng chài Vung Vieng\"]",
                Terms = "Trẻ em dưới 5 tuổi miễn phí, 5-11 tuổi giá 50%",
                CancellationPolicy = "Hủy trước 7 ngày: miễn phí, 3-7 ngày: 30%, dưới 3 ngày: 100%",
                IsFeatured = true,
                IsDomestic = true,
                IsActive = true,
                IsDeleted = false
            },
            new Tour
            {
                Name = "Tour Đà Lạt - Thành Phố Mùa Xuân",
                Description = "Khám phá Đà Lạt - thành phố mùa xuân quanh năm với cảnh quan thiên nhiên tuyệt đẹp.",
                DurationDays = 4,
                DurationNights = 3,
                BasePrice = 3800000,
                Status = TourStatus.Active,
                ThumbnailUrl = "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800",
                Highlights = "[\"Thung lũng Tình Yêu\",\"Hồ Xuân Hương\",\"Crazy House\",\"Chợ Đà Lạt\"]",
                Terms = "Trẻ em dưới 5 tuổi miễn phí",
                CancellationPolicy = "Hủy trước 5 ngày: miễn phí",
                IsFeatured = true,
                IsDomestic = true,
                IsActive = true,
                IsDeleted = false
            },
            new Tour
            {
                Name = "Tour Phú Quốc - Thiên Đường Biển Đảo",
                Description = "Trải nghiệm thiên đường biển đảo Phú Quốc với bãi biển hoang sơ và hải sản tươi ngon.",
                DurationDays = 5,
                DurationNights = 4,
                BasePrice = 7200000,
                Status = TourStatus.Active,
                ThumbnailUrl = "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800",
                Highlights = "[\"Bãi Sao\",\"Rừng Nguyên Sinh Phú Quốc\",\"Vinpearl Safari\",\"Dinh Cậu\"]",
                Terms = "Bao gồm vé vui chơi",
                CancellationPolicy = "Hủy trước 10 ngày: miễn phí",
                IsFeatured = true,
                IsDomestic = true,
                IsActive = true,
                IsDeleted = false
            },
            new Tour
            {
                Name = "Tour Hội An - Huế - Cố Đô Di Sản",
                Description = "Khám phá hai di sản văn hóa thế giới: phố cổ Hội An và cố đô Huế.",
                DurationDays = 4,
                DurationNights = 3,
                BasePrice = 5200000,
                Status = TourStatus.Active,
                ThumbnailUrl = "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800",
                Highlights = "[\"Phố Cổ Hội An\",\"Kinh Thành Huế\",\"Lăng Minh Mạng\",\"Đại Nội\"]",
                Terms = "Hướng dẫn viên tiếng Việt và Tiếng Anh",
                CancellationPolicy = "Hủy trước 7 ngày: miễn phí",
                IsFeatured = true,
                IsDomestic = true,
                IsActive = true,
                IsDeleted = false
            },
            new Tour
            {
                Name = "Tour Sapa - Mù Mịt Trong Sương",
                Description = "Khám phá Sapa với ruộng bậc thang và văn hóa các dân tộc thiểu số.",
                DurationDays = 3,
                DurationNights = 2,
                BasePrice = 2800000,
                Status = TourStatus.Active,
                ThumbnailUrl = "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800",
                Highlights = "[\"Fansipan\",\"Thác Bạc\",\"Bản Cát Cát\",\"Núi Hàm Rồng\"]",
                Terms = "Thích hợp cho gia đình và backpacker",
                CancellationPolicy = "Hủy trước 5 ngày: miễn phí",
                IsFeatured = false,
                IsDomestic = true,
                IsActive = true,
                IsDeleted = false
            },
            new Tour
            {
                Name = "Tour Nha Trang - Bờ Biển Xanh Ngắt",
                Description = "Tận hưởng bãi biển Nha Trang xanh trong với các hoạt động biển đa dạng.",
                DurationDays = 4,
                DurationNights = 3,
                BasePrice = 4800000,
                Status = TourStatus.Active,
                ThumbnailUrl = "https://images.unsplash.com/photo-1559592413-7cec4d0cae2b?w=800",
                Highlights = "[\"Vinpearl Land\",\"Tháp Bà Ponagar\",\"Hòn Mun\",\"Du Ngoạn 4 Đảo\"]",
                Terms = "Bao gồm vé vào cổng Vinpearl",
                CancellationPolicy = "Hủy trước 7 ngày: miễn phí",
                IsFeatured = true,
                IsDomestic = true,
                IsActive = true,
                IsDeleted = false
            }
        };
        context.Tours.AddRange(tours);
        await context.SaveChangesAsync();

        // ==================== TOUR ITINERARIES ====================
        var tourItineraries = new List<TourItinerary>
        {
            // Tour 1: Hạ Long
            new TourItinerary
            {
                DayNumber = 1,
                Title = "Khởi Hành & Tham Quan Vịnh",
                Content = "07:00: Đón khách tại điểm hẹn. 08:00: Khởi hành đi Hạ Long. 12:00: Check-in khách sạn. 14:00: Lên tàu tham quan vịnh Hạ Long: Hang Sửng Sốt, đảo Ti Tốp. 19:00: Ăn tối trên tàu với hải sản tươi ngon.",
                Meals = "[\"Sáng: Không bao gồm\",\"Trưa: Hải sản\",\"Tối: Hải sản\"]",
                Accommodation = "Khách sạn 4 sao Hạ Long",
                Transport = "Xe ô tô giường nằm",
                Activities = "[\"Tắm biển\",\"Chụp ảnh\",\"Ngắm cảnh hoàng hôn\"]",
                TourId = tours[0].Id,
                IsActive = true,
                IsDeleted = false
            },
            new TourItinerary
            {
                DayNumber = 2,
                Title = "Khám Phá Hang Động & Làng Chài",
                Content = "08:00: Tiếp tục tham quan vịnh: hang Đầu Gỗ, làng chài Vung Vieng. 12:00: Ăn trưa trên tàu. 14:00: Quay lại bờ, thăm thị trấn Hạ Long. 18:00: Tự do khám phá chợ đêm Hạ Long.",
                Meals = "[\"Sáng: Buffet\",\"Trưa: Hải sản\",\"Tối: Tự chi\"]",
                Accommodation = "Khách sạn 4 sao Hạ Long",
                Transport = "Tàu VIP",
                Activities = "[\"Kayaking\",\"Thăm làng chài\",\"Mua sắm\"]",
                TourId = tours[0].Id,
                IsActive = true,
                IsDeleted = false
            },
            new TourItinerary
            {
                DayNumber = 3,
                Title = "Kết Thúc Chuyến Đi",
                Content = "08:00: Ăn sáng buffet. 09:00: Check-out khách sạn. 10:00: Tham quan chùa Yên Tử (tùy chọn). 12:00: Ăn trưa. 14:00: Khởi hành về Hà Nội. 18:00: Về đến điểm đầu, kết thúc tour.",
                Meals = "[\"Sáng: Buffet\",\"Trưa: Đặc sản địa phương\",\"Tối: Không bao gồm\"]",
                Accommodation = "Không",
                Transport = "Xe ô tô",
                Activities = "[\"Tham quan chùa\",\"Mua quà lưu niệm\"]",
                TourId = tours[0].Id,
                IsActive = true,
                IsDeleted = false
            },

            // Tour 2: Đà Lạt
            new TourItinerary
            {
                DayNumber = 1,
                Title = "Khởi Hành Đến Đà Lạt",
                Content = "05:00: Khởi hành từ Sài Gòn đi Đà Lạt. 12:00: Đến Đà Lạt, nhận phòng khách sạn. 14:00: Tham quan Hồ Xuân Hương, thành phố Đà Lạt về đêm.",
                Meals = "[\"Sáng: Không bao gồm\",\"Trưa: Ăn nhẹ trên đường\",\"Tối: Đặc sản Đà Lạt\"]",
                Accommodation = "Khách sạn 3 sao Đà Lạt",
                Transport = "Xe giường nằm",
                Activities = "[\"Ngắm cảnh thành phố\",\"Chợ đêm Đà Lạt\"]",
                TourId = tours[1].Id,
                IsActive = true,
                IsDeleted = false
            },
            new TourItinerary
            {
                DayNumber = 2,
                Title = "Khám Phá Thung Lũng Tình Yêu",
                Content = "08:00: Ăn sáng. 09:00: Tham quan Thung lũng Tình Yêu, Crazy House. 12:00: Ăn trưa. 14:00: Tham quan Thác Datanla, Lavender Farm. 18:00: Ăn tối, tự do khám phá.",
                Meals = "[\"Sáng: Buffet\",\"Trưa: Đặc sản\",\"Tối: Tự chọn\"]",
                Accommodation = "Khách sạn 3 sao Đà Lạt",
                Transport = "Xe du lịch",
                Activities = "[\"Chụp ảnh\",\"Trải nghiệm mạo hiểm\"]",
                TourId = tours[1].Id,
                IsActive = true,
                IsDeleted = false
            },
            new TourItinerary
            {
                DayNumber = 3,
                Title = "Tham Quan & Về",
                Content = "08:00: Ăn sáng. 09:00: Tham quan Lang Biang, rừng thông. 12:00: Ăn trưa. 14:00: Khởi hành về Sài Gòn. 22:00: Về đến điểm đầu.",
                Meals = "[\"Sáng: Buffet\",\"Trưa: Đặc sản\",\"Tối: Không bao gồm\"]",
                Accommodation = "Không",
                Transport = "Xe giường nằm",
                Activities = "[\"Ngắm cảnh rừng thông\"]",
                TourId = tours[1].Id,
                IsActive = true,
                IsDeleted = false
            }
        };
        context.TourItineraries.AddRange(tourItineraries);

        // ==================== TOUR SERVICES ====================
        var tourServices = new List<TourService>
        {
            new TourService { ServiceName = "Khách sạn 3-4 sao", Description = "Phòng đôi tiêu chuẩn", IsIncluded = true, Category = ServiceCategory.Accommodation, TourId = tours[0].Id, IsActive = true, IsDeleted = false },
            new TourService { ServiceName = "Xe di chuyển", Description = "Xe máy lạnh giường nằm", IsIncluded = true, Category = ServiceCategory.Transport, TourId = tours[0].Id, IsActive = true, IsDeleted = false },
            new TourService { ServiceName = "Bữa ăn", Description = "3 bữa/ngày với hải sản địa phương", IsIncluded = true, Category = ServiceCategory.Meal, TourId = tours[0].Id, IsActive = true, IsDeleted = false },
            new TourService { ServiceName = "Hướng dẫn viên", Description = "Hướng dẫn viên tiếng Việt nhiệt tình", IsIncluded = true, Category = ServiceCategory.Guide, TourId = tours[0].Id, IsActive = true, IsDeleted = false },
            new TourService { ServiceName = "Vé tham quan", Description = "Tất cả vé vào cổng", IsIncluded = true, Category = ServiceCategory.Ticket, TourId = tours[0].Id, IsActive = true, IsDeleted = false },
            new TourService { ServiceName = "Bảo hiểm", Description = "Bảo hiểm du lịch", IsIncluded = true, Category = ServiceCategory.Insurance, TourId = tours[0].Id, IsActive = true, IsDeleted = false },
            new TourService { ServiceName = "Đồ uống", Description = "Nước uống cá nhân", IsIncluded = false, Category = ServiceCategory.Other, TourId = tours[0].Id, IsActive = true, IsDeleted = false },
            new TourService { ServiceName = "Tip", Description = "Tiền tip cho hướng dẫn viên và tài xế", IsIncluded = false, Category = ServiceCategory.Other, TourId = tours[0].Id, IsActive = true, IsDeleted = false }
        };
        context.TourServices.AddRange(tourServices);

        // ==================== TOUR DEPARTURES ====================
        var tourDepartures = new List<TourDeparture>();
        foreach (var tour in tours)
        {
            for (int i = 0; i < 3; i++)
            {
                var departureDate = DateTime.Now.AddDays(7 + i * 14);
                tourDepartures.Add(new TourDeparture
                {
                    DepartureDate = departureDate,
                    AvailableSeats = 20,
                    TotalSeats = 20,
                    Price = tour.BasePrice,
                    DiscountPrice = tour.BasePrice * 0.9m,
                    IsAvailable = true,
                    TourId = tour.Id,
                    IsActive = true,
                    IsDeleted = false
                });
            }
        }
        context.TourDepartures.AddRange(tourDepartures);

        // ==================== TOUR GUIDES ====================
        var tourGuides = new List<TourGuide>
        {
            new TourGuide
            {
                Name = "Nguyễn Văn A",
                Phone = "0901234567",
                Email = "guideA@karneltravels.com",
                PhotoUrl = "https://i.pravatar.cc/150?img=10",
                Specialties = "[\"Du lịch miền Bắc\",\"Lịch sử\"]",
                YearsExperience = 5,
                IsAvailable = true,
                IsActive = true,
                IsDeleted = false
            },
            new TourGuide
            {
                Name = "Trần Thị B",
                Phone = "0902234567",
                Email = "guideB@karneltravels.com",
                PhotoUrl = "https://i.pravatar.cc/150?img=11",
                Specialties = "[\"Du lịch miền Trung\",\"Văn hóa\"]",
                YearsExperience = 7,
                IsAvailable = true,
                IsActive = true,
                IsDeleted = false
            },
            new TourGuide
            {
                Name = "Lê Văn C",
                Phone = "0903234567",
                Email = "guideC@karneltravels.com",
                PhotoUrl = "https://i.pravatar.cc/150?img=12",
                Specialties = "[\"Du lịch miền Nam\",\"Thiên nhiên\"]",
                YearsExperience = 4,
                IsAvailable = true,
                IsActive = true,
                IsDeleted = false
            },
            new TourGuide
            {
                Name = "Phạm Thị D",
                Phone = "0904234567",
                Email = "guideD@karneltravels.com",
                PhotoUrl = "https://i.pravatar.cc/150?img=13",
                Specialties = "[\"Du lịch biển đảo\",\"Ẩm thực\"]",
                YearsExperience = 6,
                IsAvailable = true,
                IsActive = true,
                IsDeleted = false
            }
        };
        context.TourGuides.AddRange(tourGuides);
        await context.SaveChangesAsync();

        // ==================== VEHICLE TYPES ====================
        var vehicleTypes = new List<VehicleType>
        {
            new VehicleType { VehicleTypeId = Guid.NewGuid(), Name = "Xe 16 chỗ", Description = "Xe Toyota Hiace 16 chỗ", Icon = "van", IsActive = true },
            new VehicleType { VehicleTypeId = Guid.NewGuid(), Name = "Xe 29 chỗ", Description = "Xe Thaco 29 chỗ", Icon = "bus", IsActive = true },
            new VehicleType { VehicleTypeId = Guid.NewGuid(), Name = "Xe 45 chỗ", Description = "Xe Hyundai County 45 chỗ", Icon = "bus", IsActive = true },
            new VehicleType { VehicleTypeId = Guid.NewGuid(), Name = "Xe giường nằm", Description = "Xe giường nằm 40-45 chỗ", Icon = "sleeper-bus", IsActive = true },
            new VehicleType { VehicleTypeId = Guid.NewGuid(), Name = "Xe limousine", Description = "Xe limousine 9-12 chỗ", Icon = "limousine", IsActive = true },
            new VehicleType { VehicleTypeId = Guid.NewGuid(), Name = "Máy bay", Description = "Các chuyến bay nội địa", Icon = "plane", IsActive = true }
        };
        context.VehicleTypes.AddRange(vehicleTypes);

        // ==================== TRANSPORT PROVIDERS ====================
        var transportProviders = new List<TransportProvider>
        {
            new TransportProvider
            {
                ProviderId = Guid.NewGuid(),
                Name = "Futa Bus Lines",
                Description = "Hãng xe khách nổi tiếng với dịch vụ chất lượng",
                ContactPhone = "1900 6915",
                ContactEmail = "info@futa.com.vn",
                ContactAddress = "Hà Nội",
                Website = "https://futabus.vn",
                LogoUrl = "https://example.com/logo/futa.png",
                IsActive = true
            },
            new TransportProvider
            {
                ProviderId = Guid.NewGuid(),
                Name = "Mai Linh Group",
                Description = "Hãng taxi và xe du lịch hàng đầu Việt Nam",
                ContactPhone = "1900 6699",
                ContactEmail = "contact@mai-linh.com",
                ContactAddress = "Hà Nội",
                Website = "https://mailinh.vn",
                LogoUrl = "https://example.com/logo/mailinh.png",
                IsActive = true
            },
            new TransportProvider
            {
                ProviderId = Guid.NewGuid(),
                Name = "Vietnam Airlines",
                Description = "Hãng hàng không quốc gia Việt Nam",
                ContactPhone = "1900 1100",
                ContactEmail = "info@vietnamairlines.com",
                ContactAddress = "Hà Nội",
                Website = "https://vietnamairlines.com",
                LogoUrl = "https://example.com/logo/vna.png",
                IsActive = true
            },
            new TransportProvider
            {
                ProviderId = Guid.NewGuid(),
                Name = "VietJet Air",
                Description = "Hãng hàng không giá rẻ",
                ContactPhone = "1900 1886",
                ContactEmail = "info@vietjetair.com",
                ContactAddress = "TP. Hồ Chí Minh",
                Website = "https://vietjetair.com",
                LogoUrl = "https://example.com/logo/vjet.png",
                IsActive = true
            }
        };
        context.TransportProviders.AddRange(transportProviders);
        await context.SaveChangesAsync();

        // ==================== VEHICLES ====================
        var vehicles = new List<Vehicle>();
        var providers = transportProviders.ToList();
        var vTypes = vehicleTypes.ToList();

        for (int i = 1; i <= 10; i++)
        {
            vehicles.Add(new Vehicle
            {
                VehicleId = Guid.NewGuid(),
                Name = $"Xe {providers[0].Name} #{i:D3}",
                LicensePlate = $"51A-{12345 + i:D4}",
                Capacity = 45,
                VehicleTypeId = vTypes[2].VehicleTypeId,
                ProviderId = providers[0].ProviderId,
                Description = "Xe giường nằm cao cấp, tiện nghi",
                ImageUrl = "https://images.unsplash.com/photo-1534237710431-e2fc698436d0?w=800",
                Amenities = "[\"WiFi\",\"AC\",\"USB\",\"Water\"]",
                Status = VehicleStatus.Available,
                IsActive = true,
                IsDeleted = false
            });
        }

        for (int i = 1; i <= 5; i++)
        {
            vehicles.Add(new Vehicle
            {
                VehicleId = Guid.NewGuid(),
                Name = $"Xe limousine #{i:D2}",
                LicensePlate = $"51C-{54321 + i:D4}",
                Capacity = 9,
                VehicleTypeId = vTypes[4].VehicleTypeId,
                ProviderId = providers[1].ProviderId,
                Description = "Xe limousine 9 chỗ sang trọng",
                ImageUrl = "https://images.unsplash.com/photo-1549317661-bd32c8ce0db2?w=800",
                Amenities = "[\"WiFi\",\"AC\",\"USB\",\"Water\",\"TV\"]",
                Status = VehicleStatus.Available,
                IsActive = true,
                IsDeleted = false
            });
        }
        context.Vehicles.AddRange(vehicles);

        // ==================== ROUTES ====================
        var routes = new List<Route>
        {
            new Route { RouteId = Guid.NewGuid(), DepartureLocation = "TP. Hồ Chí Minh", ArrivalLocation = "Đà Lạt", RouteName = "Tuyến Sài Gòn - Đà Lạt", Distance = 300, Description = "Tuyến xe khách Sài Gòn - Đà Lạt", IsActive = true },
            new Route { RouteId = Guid.NewGuid(), DepartureLocation = "TP. Hồ Chí Minh", ArrivalLocation = "Nha Trang", RouteName = "Tuyến Sài Gòn - Nha Trang", Distance = 450, Description = "Tuyến xe khách Sài Gòn - Nha Trang", IsActive = true },
            new Route { RouteId = Guid.NewGuid(), DepartureLocation = "Hà Nội", ArrivalLocation = "Sapa", RouteName = "Tuyến Hà Nội - Sapa", Distance = 320, Description = "Tuyến xe khách Hà Nội - Sapa", IsActive = true },
            new Route { RouteId = Guid.NewGuid(), DepartureLocation = "TP. Hồ Chí Minh", ArrivalLocation = "Phú Quốc", RouteName = "Tuyến Sài Gòn - Phú Quốc", Distance = 0, Description = "Tuyến bay Sài Gòn - Phú Quốc", IsActive = true },
            new Route { RouteId = Guid.NewGuid(), DepartureLocation = "Hà Nội", ArrivalLocation = "Hạ Long", RouteName = "Tuyến Hà Nội - Hạ Long", Distance = 170, Description = "Tuyến xe khách Hà Nội - Hạ Long", IsActive = true }
        };
        context.Routes.AddRange(routes);
        await context.SaveChangesAsync();

        // ==================== SCHEDULES ====================
        var schedules = new List<Schedule>();
        var vehicleList = vehicles.Take(5).ToList();
        var routeList = routes.Take(5).ToList();

        for (int i = 0; i < vehicleList.Count; i++)
        {
            var departureTime = TimeSpan.FromHours(7 + i * 3);
            var routeDistance = routeList[i].Distance ?? 0;
            var arrivalTime = departureTime.Add(TimeSpan.FromHours(routeDistance > 0 ? routeDistance / 60 : 2));

            schedules.Add(new Schedule
            {
                ScheduleId = Guid.NewGuid(),
                VehicleId = vehicleList[i].VehicleId,
                RouteId = routeList[i].RouteId,
                DepartureTime = departureTime,
                ArrivalTime = arrivalTime,
                Price = (decimal)(routeDistance * 3000),
                AvailableSeats = vehicleList[i].Capacity,
                TotalSeats = vehicleList[i].Capacity,
                OperatingDays = "[\"Mon\",\"Tue\",\"Wed\",\"Thu\",\"Fri\",\"Sat\",\"Sun\"]",
                Notes = "Có điều hòa, wifi miễn phí",
                Status = ScheduleStatus.Active,
                IsActive = true,
                IsDeleted = false
            });
        }
        context.Schedules.AddRange(schedules);

        // ==================== REVIEWS ====================
        var reviews = new List<Review>
        {
            new Review
            {
                Rating = 5,
                Title = "Chuyến đi tuyệt vời!",
                Content = "Tôi rất hài lòng với chuyến đi Hạ Long. Cảnh vịnh đẹp không tả được, tàu VIP rất thoải mái.",
                Type = ReviewType.TouristSpot,
                UserId = users[3].Id,
                TouristSpotId = touristSpots[0].Id,
                IsActive = true,
                IsDeleted = false
            },
            new Review
            {
                Rating = 4,
                Title = "Khách sạn tốt, vị trí thuận tiện",
                Content = "InterContinental là khách sạn tuyệt vời với dịch vụ chuyên nghiệp. Vị trí gần trung tâm.",
                Type = ReviewType.Hotel,
                UserId = users[4].Id,
                HotelId = hotels[0].Id,
                IsActive = true,
                IsDeleted = false
            },
            new Review
            {
                Rating = 5,
                Title = "Hải sản tươi ngon!",
                Content = "Nhà hàng The Deck có hải sản rất tươi, view đẹp, nhân viên nhiệt tình.",
                Type = ReviewType.Restaurant,
                UserId = users[5].Id,
                RestaurantId = restaurants[0].Id,
                IsActive = true,
                IsDeleted = false
            },
            new Review
            {
                Rating = 5,
                Title = "Kỳ nghỉ hoàn hảo",
                Content = "Six Senses là khu nghỉ dưỡng tuyệt vời với bãi biển riêng, dịch vụ cao cấp.",
                Type = ReviewType.Resort,
                UserId = users[6].Id,
                ResortId = resorts[0].Id,
                IsActive = true,
                IsDeleted = false
            },
            new Review
            {
                Rating = 4,
                Title = "Tour chất lượng",
                Content = "Tour Đà Lạt được tổ chức tốt, hướng dẫn viên nhiệt tình, khách sạn sạch sẽ.",
                Type = ReviewType.Tour,
                UserId = users[3].Id,
                TourPackageId = tourPackages[1].Id,
                IsActive = true,
                IsDeleted = false
            }
        };
        context.Reviews.AddRange(reviews);
        await context.SaveChangesAsync();

        // ==================== FAVORITES ====================
        var favorites = new List<Favorite>
        {
            new Favorite
            {
                ItemType = FavoriteType.TouristSpot,
                UserId = users[3].Id,
                TouristSpotId = touristSpots[0].Id,
                IsActive = true,
                IsDeleted = false
            },
            new Favorite
            {
                ItemType = FavoriteType.Hotel,
                UserId = users[3].Id,
                HotelId = hotels[0].Id,
                IsActive = true,
                IsDeleted = false
            },
            new Favorite
            {
                ItemType = FavoriteType.Restaurant,
                UserId = users[4].Id,
                RestaurantId = restaurants[1].Id,
                IsActive = true,
                IsDeleted = false
            },
            new Favorite
            {
                ItemType = FavoriteType.Resort,
                UserId = users[5].Id,
                ResortId = resorts[0].Id,
                IsActive = true,
                IsDeleted = false
            },
            new Favorite
            {
                ItemType = FavoriteType.Tour,
                UserId = users[6].Id,
                TourPackageId = tourPackages[0].Id,
                IsActive = true,
                IsDeleted = false
            }
        };
        context.Favorites.AddRange(favorites);

        // ==================== CONTACTS ====================
        var contacts = new List<Contact>
        {
            new Contact
            {
                FullName = "Nguyễn Văn Khách",
                Email = "khachhang1@email.com",
                Subject = "Tư vấn tour Hạ Long",
                PhoneNumber = "0901111222",
                ServiceType = "Tour",
                ExpectedDate = DateTime.Now.AddDays(14),
                ParticipantCount = 4,
                MessageContent = "Tôi muốn được tư vấn về tour Hạ Long 3 ngày 2 đêm cho 4 người vào dịp cuối tuần sau.",
                Status = ContactStatus.Unread,
                IsDeleted = false
            },
            new Contact
            {
                FullName = "Trần Thị Hương",
                Email = "huong.tran@email.com",
                Subject = "Đặt phòng khách sạn",
                PhoneNumber = "0903333444",
                ServiceType = "Hotel",
                ParticipantCount = 2,
                MessageContent = "Tôi muốn đặt phòng Deluxe tại InterContinental Hanoi Westlake vào ngày 20/3.",
                Status = ContactStatus.Read,
                ReplyMessage = "Cảm ơn bạn đã liên hệ. Chúng tôi sẽ gửi báo giá chi tiết qua email.",
                RepliedAt = DateTime.Now.AddHours(-2),
                IsDeleted = false
            },
            new Contact
            {
                FullName = "Lê Minh Đức",
                Email = "duc.le@email.com",
                Subject = "Phản hồi về dịch vụ",
                PhoneNumber = "0905555666",
                MessageContent = "Tôi rất hài lòng với dịch vụ của Karnel Travels. Cảm ơn đội ngũ đã tổ chức chuyến đi tuyệt vời!",
                Status = ContactStatus.Replied,
                ReplyMessage = "Cảm ơn bạn đã tin tưởng và phản hồi tích cực. Chúc bạn luôn có những chuyến đi vui vẻ!",
                RepliedAt = DateTime.Now.AddDays(-1),
                IsDeleted = false
            }
        };
        context.Contacts.AddRange(contacts);

        // ==================== BOOKINGS ====================
        var bookings = new List<Booking>
        {
            new Booking
            {
                BookingCode = "BK" + DateTime.Now.ToString("yyyyMMdd") + "001",
                Type = BookingType.Tour,
                Status = BookingStatus.Confirmed,
                ServiceDate = DateTime.Now.AddDays(7),
                EndDate = DateTime.Now.AddDays(10),
                Quantity = 2,
                TotalAmount = 9000000,
                DiscountAmount = 1000000,
                FinalAmount = 8000000,
                PaymentStatus = PaymentStatus.Paid,
                PaymentMethod = "Bank Transfer",
                PaidAt = DateTime.Now.AddDays(-1),
                ContactName = "Nguyễn Văn Khách",
                ContactEmail = "khachhang1@email.com",
                ContactPhone = "0901111222",
                UserId = users[3].Id,
                TourPackageId = tourPackages[0].Id,
                IsActive = true,
                IsDeleted = false
            },
            new Booking
            {
                BookingCode = "BK" + DateTime.Now.ToString("yyyyMMdd") + "002",
                Type = BookingType.Hotel,
                Status = BookingStatus.Confirmed,
                ServiceDate = DateTime.Now.AddDays(14),
                EndDate = DateTime.Now.AddDays(17),
                Quantity = 1,
                TotalAmount = 10500000,
                FinalAmount = 10500000,
                PaymentStatus = PaymentStatus.Paid,
                PaymentMethod = "Credit Card",
                PaidAt = DateTime.Now.AddDays(-2),
                ContactName = "Trần Thị Hương",
                ContactEmail = "huong.tran@email.com",
                ContactPhone = "0903333444",
                UserId = users[4].Id,
                HotelId = hotels[0].Id,
                HotelRoomId = hotelRooms[1].Id,
                IsActive = true,
                IsDeleted = false
            },
            new Booking
            {
                BookingCode = "BK" + DateTime.Now.ToString("yyyyMMdd") + "003",
                Type = BookingType.Tour,
                Status = BookingStatus.Pending,
                ServiceDate = DateTime.Now.AddDays(21),
                EndDate = DateTime.Now.AddDays(24),
                Quantity = 4,
                TotalAmount = 14000000,
                DiscountAmount = 1400000,
                FinalAmount = 12600000,
                PaymentStatus = PaymentStatus.Pending,
                ContactName = "Lê Minh Đức",
                ContactEmail = "duc.le@email.com",
                ContactPhone = "0905555666",
                UserId = users[5].Id,
                TourPackageId = tourPackages[2].Id,
                PromotionId = promotions[0].Id,
                IsActive = true,
                IsDeleted = false
            },
            new Booking
            {
                BookingCode = "BK" + DateTime.Now.ToString("yyyyMMdd") + "004",
                Type = BookingType.Resort,
                Status = BookingStatus.Completed,
                ServiceDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(-7),
                Quantity = 1,
                TotalAmount = 45000000,
                FinalAmount = 45000000,
                PaymentStatus = PaymentStatus.Paid,
                PaymentMethod = "Bank Transfer",
                PaidAt = DateTime.Now.AddDays(-15),
                ContactName = "Phạm Thị Mai",
                ContactEmail = "mai.pham@email.com",
                ContactPhone = "0907777888",
                UserId = users[6].Id,
                ResortId = resorts[0].Id,
                ResortRoomId = resortRooms[1].Id,
                IsActive = true,
                IsDeleted = false
            },
            new Booking
            {
                BookingCode = "BK" + DateTime.Now.ToString("yyyyMMdd") + "005",
                Type = BookingType.Transport,
                Status = BookingStatus.Confirmed,
                ServiceDate = DateTime.Now.AddDays(3),
                Quantity = 1,
                TotalAmount = 350000,
                FinalAmount = 350000,
                PaymentStatus = PaymentStatus.Paid,
                PaymentMethod = "MoMo",
                PaidAt = DateTime.Now.AddHours(-5),
                ContactName = "Vũ Thị Lan",
                ContactEmail = "lan.vu@email.com",
                ContactPhone = "0909999000",
                UserId = users[3].Id,
                TransportId = transports[3].Id,
                IsActive = true,
                IsDeleted = false
            }
        };
        context.Bookings.AddRange(bookings);

        // Save all remaining changes
        await context.SaveChangesAsync();
    }
}
