import { Link } from 'react-router-dom';
import { useState } from 'react';
import { Star, MapPin, Calendar, Ticket, Heart } from 'lucide-react';
import favoriteService from '@/services/favoriteService';

/**
 * SpotCard - Card component for displaying tourist spot
 * Use in grid layouts for consistent card appearance
 */
const SpotCard = ({ spot, isFavorite: externalIsFavorite, onFavoriteToggle }) => {
  const {
    spotId,
    name,
    description,
    region,
    type,
    city,
    images,
    ticketPrice,
    rating,
    reviewCount,
    bestTime,
  } = spot;

  const [isFavorite, setIsFavorite] = useState(externalIsFavorite || false);
  const [favoriteLoading, setFavoriteLoading] = useState(false);

  const imageUrl = images?.[0] || "https://images.unsplash.com/photo-1506929562872-bb421503ef21?w=800&q=80";

  const formatPrice = (price) => {
    if (!price || price === 0) return 'Free';
    return `${price.toLocaleString('en-US')}₫`;
  };

  const regionColors = {
    North: 'bg-blue-100 text-blue-700',
    Central: 'bg-amber-100 text-amber-700',
    South: 'bg-green-100 text-green-700',
  };

  const handleFavoriteClick = async (e) => {
    e.preventDefault();
    setFavoriteLoading(true);
    try {
      if (isFavorite) {
        await favoriteService.removeByItem('TouristSpot', spotId);
      } else {
        await favoriteService.addFavorite('TouristSpot', spotId);
      }
      setIsFavorite(!isFavorite);
      if (onFavoriteToggle) {
        onFavoriteToggle(spotId, !isFavorite);
      }
    } catch (err) {
      console.error('Error toggling favorite:', err);
    } finally {
      setFavoriteLoading(false);
    }
  };

  return (
    <div className="group bg-white rounded-xl overflow-hidden shadow-md hover:shadow-xl transition-all duration-300">
      {/* Image */}
      <div className="relative h-56 overflow-hidden">
        <img
          src={imageUrl}
          alt={name}
          className="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
        />
        
        {/* Badges */}
        <div className="absolute top-3 left-3 flex gap-2">
          <span className={`px-2 py-1 rounded-lg text-xs font-medium ${regionColors[region] || 'bg-gray-100 text-gray-700'}`}>
            {region}
          </span>
          <span className="px-2 py-1 bg-white/90 backdrop-blur-sm rounded-lg text-xs font-medium text-gray-700">
            {type}
          </span>
        </div>

        {/* Favorite Button */}
        <button
          onClick={handleFavoriteClick}
          disabled={favoriteLoading}
          className={`absolute top-3 right-3 w-9 h-9 bg-white/90 backdrop-blur-sm rounded-full flex items-center justify-center transition-colors ${
            isFavorite ? 'text-red-500' : 'text-gray-400 hover:text-red-500'
          }`}
        >
          <Heart className={`w-5 h-5 ${isFavorite ? 'fill-current' : ''}`} />
        </button>
      </div>

      {/* Content */}
      <div className="p-4">
        {/* Location */}
        <div className="flex items-center gap-1 text-sm text-gray-500 mb-2">
          <MapPin className="w-4 h-4" />
          <span>{city || region}</span>
        </div>

        {/* Title */}
        <h3 className="font-semibold text-lg text-gray-800 mb-2 line-clamp-1 group-hover:text-primary transition-colors">
          {name}
        </h3>

        {/* Description */}
        <p className="text-gray-500 text-sm mb-3 line-clamp-2">
          {description}
        </p>

        {/* Info Row */}
        <div className="flex items-center justify-between mb-3">
          {/* Rating */}
          <div className="flex items-center gap-1">
            <Star className="w-4 h-4 fill-yellow-400 text-yellow-400" />
            <span className="font-medium text-sm">{rating?.toFixed(1) || '0.0'}</span>
            <span className="text-gray-400 text-xs">({reviewCount || 0})</span>
          </div>

          {/* Price */}
          <div className="text-right">
            <span className="text-primary font-bold">{formatPrice(ticketPrice)}</span>
          </div>
        </div>

        {/* Best Time */}
        {bestTime && (
          <div className="flex items-center gap-1 text-sm text-gray-500 mb-4">
            <Calendar className="w-4 h-4" />
            <span>Best time: {bestTime}</span>
          </div>
        )}

        {/* Actions */}
        <div className="flex gap-2">
          <Link
            to={`/info/tourist-spots/${spotId}`}
            className="flex-1 px-4 py-2 bg-gray-100 text-gray-700 text-center rounded-lg font-medium hover:bg-gray-200 transition-colors"
          >
            View Details
          </Link>
          <Link
            to={`/search?spot=${spotId}`}
            className="flex-1 px-4 py-2 bg-primary text-white text-center rounded-lg font-medium hover:bg-primary/90 transition-colors"
          >
            Book Now
          </Link>
        </div>
      </div>
    </div>
  );
};

export default SpotCard;
