namespace KarnelTravels.API.Entities;

public enum CuisineType
{
    Vietnamese = 0,
    Chinese = 1,
    Japanese = 2,
    Korean = 3,
    Thai = 4,
    Indian = 5,
    Italian = 6,
    French = 7,
    American = 8,
    Seafood = 9,
    BBQ = 10,
    Buffet = 11,
    Cafe = 12,
    Bar = 13,
    FastFood = 14,
    Vegetarian = 15,
    Other = 16
}

public enum PriceRange
{
    Budget = 0,      // Dưới 100k
    MidRange = 1,   // 100k - 300k
    HighEnd = 2,     // 300k - 500k
    Luxury = 3,      // 500k - 1tr
    Premium = 4      // Trên 1tr
}

public enum RestaurantStyle
{
    Restaurant = 0,
    Cafe = 1,
    Bar = 2,
    Pub = 3,
    Bakery = 4,
    IceCream = 5,
    TeaHouse = 6
}
