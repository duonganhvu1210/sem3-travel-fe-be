import { Star, User, Clock, ThumbsUp, Flag } from 'lucide-react';

const ReviewList = ({ 
  reviews = [], 
  loading = false,
  onReply,
  onReport,
  showActions = true
}) => {
  const renderStars = (rating) => {
    return [...Array(5)].map((_, i) => (
      <Star 
        key={i} 
        className={`w-4 h-4 ${i < Math.round(rating) ? 'text-yellow-400 fill-yellow-400' : 'text-gray-300'}`} 
      />
    ));
  };

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN', { 
      day: 'numeric', 
      month: 'long', 
      year: 'numeric' 
    });
  };

  if (loading) {
    return (
      <div className="space-y-4">
        {[1, 2, 3].map((i) => (
          <div key={i} className="animate-pulse">
            <div className="flex items-center gap-3 mb-3">
              <div className="w-10 h-10 bg-gray-200 rounded-full"></div>
              <div className="space-y-2">
                <div className="h-4 w-32 bg-gray-200 rounded"></div>
                <div className="h-3 w-24 bg-gray-200 rounded"></div>
              </div>
            </div>
            <div className="h-16 bg-gray-200 rounded"></div>
          </div>
        ))}
      </div>
    );
  }

  if (reviews.length === 0) {
    return (
      <div className="text-center py-8">
        <Star className="w-12 h-12 text-gray-300 mx-auto mb-3" />
        <p className="text-gray-500">No reviews yet</p>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {reviews.map((review) => (
        <div key={review.reviewId || review.id} className="border-b border-gray-100 pb-4 last:border-0">
          <div className="flex items-start justify-between">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-teal-100 rounded-full flex items-center justify-center">
                <User className="w-5 h-5 text-teal-600" />
              </div>
              <div>
                <h4 className="font-medium text-gray-900">
                  {review.userName || review.user?.fullName || 'Khách hàng'}
                </h4>
                <div className="flex items-center gap-2 text-sm text-gray-500">
                  <div className="flex">{renderStars(review.rating)}</div>
                  <span>•</span>
                  <span className="flex items-center gap-1">
                    <Clock className="w-3 h-3" />
                    {formatDate(review.createdAt)}
                  </span>
                </div>
              </div>
            </div>
            
            {showActions && (
              <div className="flex items-center gap-2">
                {onReply && (
                  <button
                    onClick={() => onReply(review)}
                    className="text-sm text-gray-500 hover:text-teal-600 flex items-center gap-1"
                  >
                    <ThumbsUp className="w-4 h-4" />
                    Trả lời
                  </button>
                )}
                {onReport && (
                  <button
                    onClick={() => onReport(review)}
                    className="text-sm text-gray-500 hover:text-red-600 flex items-center gap-1"
                  >
                    <Flag className="w-4 h-4" />
                    Báo cáo
                  </button>
                )}
              </div>
            )}
          </div>

          <div className="mt-3">
            <p className="text-gray-700">{review.comment || review.content}</p>
          </div>

          {review.reply && (
            <div className="mt-3 ml-6 p-3 bg-gray-50 rounded-lg">
              <p className="text-sm font-medium text-teal-600 mb-1">Phản hồi:</p>
              <p className="text-sm text-gray-600">{review.reply}</p>
            </div>
          )}

          {review.images && review.images.length > 0 && (
            <div className="mt-3 flex gap-2">
              {review.images.map((img, idx) => (
                <img 
                  key={idx}
                  src={img}
                  alt={`Review ${idx + 1}`}
                  className="w-16 h-16 object-cover rounded-lg"
                />
              ))}
            </div>
          )}
        </div>
      ))}
    </div>
  );
};

export default ReviewList;
