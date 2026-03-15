import { useState, useEffect } from 'react';
import { useAuth } from '@/context/AuthContext/AuthContext';
import { profileService } from '@/services/profileService';
import { Loader2, User, MapPin, Shield, Activity, Check, Mail } from 'lucide-react';
import AvatarSection from './components/AvatarSection';
import InformationForm from './components/InformationForm';
import AddressManager from './components/AddressManager';
import SecurityTab from './components/SecurityTab';
import ActivityLog from './components/ActivityLog';

const tabs = [
  { id: 'info', label: 'Thông tin chung', icon: User },
  { id: 'addresses', label: 'Địa chỉ', icon: MapPin },
  { id: 'security', label: 'Bảo mật', icon: Shield },
  { id: 'activities', label: 'Hoạt động', icon: Activity },
];

const ProfilePage = () => {
  const { user, setUser } = useAuth();
  const [activeTab, setActiveTab] = useState('info');
  const [isLoading, setIsLoading] = useState(true);
  const [profile, setProfile] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => {
    loadProfile();
  }, []);

  const loadProfile = async () => {
    setIsLoading(true);
    const response = await profileService.getProfile();
    if (response.success) {
      setProfile(response.data);
    } else {
      setError(response.message || 'Không thể tải thông tin hồ sơ');
    }
    setIsLoading(false);
  };

  const handleProfileUpdate = (updatedProfile) => {
    setProfile(updatedProfile);
    if (setUser) {
      setUser((prev) => ({
        ...prev,
        ...updatedProfile,
      }));
    }
  };

  if (isLoading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <Loader2 className="w-8 h-8 animate-spin text-teal-600" />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-white to-cyan-50 py-8">
      <div className="container mx-auto px-4 max-w-6xl">
        {/* Header */}
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Hồ sơ cá nhân</h1>
          <p className="text-gray-600 mt-2">Quản lý thông tin cá nhân và cài đặt tài khoản</p>
        </div>

        {/* Email Verification Alert */}
        {profile && !profile.isEmailVerified && (
          <div className="mb-6 p-4 bg-amber-50 border border-amber-200 rounded-lg flex items-center justify-between">
            <div className="flex items-center gap-3">
              <Mail className="w-5 h-5 text-amber-600" />
              <div>
                <p className="font-medium text-amber-800">Email chưa được xác thực</p>
                <p className="text-sm text-amber-600">Vui lòng xác thực email để sử dụng đầy đủ tính năng</p>
              </div>
            </div>
            <button className="px-4 py-2 bg-amber-600 text-white rounded-lg hover:bg-amber-700 transition-colors text-sm font-medium">
              Xác thực ngay
            </button>
          </div>
        )}

        {/* Email Verified Badge */}
        {profile && profile.isEmailVerified && (
          <div className="mb-6 p-4 bg-green-50 border border-green-200 rounded-lg flex items-center gap-3">
            <Check className="w-5 h-5 text-green-600" />
            <p className="font-medium text-green-800">Email đã được xác thực</p>
          </div>
        )}

        <div className="flex flex-col lg:flex-row gap-6">
          {/* Sidebar Navigation */}
          <div className="lg:w-64 flex-shrink-0">
            <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
              <nav className="p-2">
                {tabs.map((tab) => {
                  const Icon = tab.icon;
                  return (
                    <button
                      key={tab.id}
                      onClick={() => setActiveTab(tab.id)}
                      className={`w-full flex items-center gap-3 px-4 py-3 rounded-lg text-left transition-all ${
                        activeTab === tab.id
                          ? 'bg-gradient-to-r from-teal-500 to-cyan-600 text-white shadow-md'
                          : 'text-gray-600 hover:bg-gray-50'
                      }`}
                    >
                      <Icon className="w-5 h-5" />
                      <span className="font-medium">{tab.label}</span>
                    </button>
                  );
                })}
              </nav>
            </div>
          </div>

          {/* Main Content */}
          <div className="flex-1">
            {error && (
              <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg text-red-700">
                {error}
              </div>
            )}

            {activeTab === 'info' && (
              <div className="space-y-6">
                <AvatarSection profile={profile} onAvatarUpdate={handleProfileUpdate} />
                <InformationForm profile={profile} onProfileUpdate={handleProfileUpdate} />
              </div>
            )}

            {activeTab === 'addresses' && <AddressManager />}

            {activeTab === 'security' && (
              <SecurityTab onPasswordChange={() => {}} />
            )}

            {activeTab === 'activities' && <ActivityLog />}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProfilePage;
