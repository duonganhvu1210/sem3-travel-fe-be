import api from './api';

// ==================== HOME PAGE SERVICES ====================

/**
 * Lấy dữ liệu tổng hợp cho trang chủ
 * @returns {Promise<any>}
 */
export const getHomeData = async () => {
  try {
    const response = await api.get('/home');
    return response.data;
  } catch (error) {
    console.error('Error fetching home data:', error);
    throw error;
  }
};

// ==================== TYPES (for reference) ====================

/*
  // Banner Type
  {
    id: string,
    title: string,
    subtitle: string,
    imageUrl: string,
    videoUrl: string | null,
    ctaText: string,
    ctaLink: string,
    isActive: boolean
  }

  // CompanyInfo Type
  {
    id: string,
    companyName: string,
    tagline: string,
    description: string,
    aboutTitle: string,
    aboutPoints: string[],
    aboutImage: string | null,
    features: { icon: string, title: string, description: string }[]
  }

  // TouristSpotCard Type
  {
    spotId: string,
    name: string,
    description: string | null,
    region: string,
    type: string,
    address: string | null,
    city: string | null,
    imageUrl: string | null,
    ticketPrice: number | null,
    rating: number,
    reviewCount: number,
    bestTime: string | null,
    isFeatured: boolean
  }

  // ServiceCategory Type
  {
    id: string,
    name: string,
    icon: string,
    description: string,
    itemCount: number,
    link: string,
    color: string
  }

  // ContactInfo Type
  {
    companyName: string,
    hotline: string | null,
    phone: string | null,
    email: string | null,
    address: string | null,
    website: string | null,
    workingHours: string | null
  }

  // SocialLinks Type
  {
    facebook: string | null,
    instagram: string | null,
    youtube: string | null,
    zalo: string | null,
    twitter: string | null,
    tiktok: string | null
  }

  // HomeData Response Type
  {
    success: boolean,
    message: string,
    data: {
      banner: Banner,
      companyInfo: CompanyInfo,
      featuredSpots: TouristSpotCard[],
      serviceCategories: ServiceCategory[],
      contactInfo: ContactInfo,
      socialLinks: SocialLinks
    }
  }
*/
