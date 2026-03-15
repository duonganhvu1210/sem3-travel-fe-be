import { useState, useRef } from 'react';
import { profileService } from '@/services/profileService';
import { Camera, Loader2, User } from 'lucide-react';

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000';

const AvatarSection = ({ profile, onAvatarUpdate }) => {
  const [isUploading, setIsUploading] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' });
  const fileInputRef = useRef(null);

  const handleFileChange = async (e) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setIsUploading(true);
    setMessage({ type: '', text: '' });

    const response = await profileService.uploadAvatar(file);

    if (response.success) {
      setMessage({ type: 'success', text: 'Cập nhật ảnh đại diện thành công!' });
      if (onAvatarUpdate) {
        onAvatarUpdate({ ...profile, avatar: response.data });
      }
    } else {
      setMessage({ type: 'error', text: response.message || 'Cập nhật ảnh thất bại' });
    }

    setIsUploading(false);
    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
  };

  const getAvatarUrl = () => {
    if (!profile?.avatar) return null;
    if (profile.avatar.startsWith('http')) return profile.avatar;
    return `${API_URL}${profile.avatar}`;
  };

  const avatarUrl = getAvatarUrl();

  return (
    <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
      <h2 className="text-lg font-semibold text-gray-900 mb-4">Ảnh đại diện</h2>

      <div className="flex items-center gap-6">
        {/* Avatar Preview */}
        <div className="relative">
          <div className="w-32 h-32 rounded-full overflow-hidden bg-gradient-to-br from-teal-100 to-cyan-100 flex items-center justify-center border-4 border-white shadow-lg">
            {avatarUrl ? (
              <img
                src={avatarUrl}
                alt="Avatar"
                className="w-full h-full object-cover"
              />
            ) : (
              <User className="w-16 h-16 text-teal-600" />
            )}
          </div>

          {/* Upload Overlay */}
          <label
            htmlFor="avatar-upload"
            className="absolute inset-0 flex items-center justify-center bg-black/50 rounded-full cursor-pointer opacity-0 hover:opacity-100 transition-opacity"
          >
            {isUploading ? (
              <Loader2 className="w-8 h-8 text-white animate-spin" />
            ) : (
              <Camera className="w-8 h-8 text-white" />
            )}
          </label>
          <input
            ref={fileInputRef}
            id="avatar-upload"
            type="file"
            accept="image/jpeg,image/jpg,image/png,image/gif,image/webp"
            className="hidden"
            onChange={handleFileChange}
            disabled={isUploading}
          />
        </div>

        {/* Info */}
        <div className="flex-1">
          <h3 className="font-medium text-gray-900">{profile?.fullName || 'Người dùng'}</h3>
          <p className="text-sm text-gray-500 mt-1">JPG, PNG, GIF hoặc WebP. Tối đa 5MB.</p>

          {/* Message */}
          {message.text && (
            <div
              className={`mt-3 p-2 rounded-lg text-sm ${
                message.type === 'success'
                  ? 'bg-green-50 text-green-700'
                  : 'bg-red-50 text-red-700'
              }`}
            >
              {message.text}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default AvatarSection;
