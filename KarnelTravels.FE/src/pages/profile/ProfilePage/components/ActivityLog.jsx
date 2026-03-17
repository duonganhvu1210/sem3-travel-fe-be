import { useState, useEffect } from 'react';
import { profileService } from '@/services/profileService';
import { Loader2, Activity, Clock, Monitor } from 'lucide-react';

const ActivityLog = () => {
  const [activities, setActivities] = useState([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadActivities();
  }, []);

  const loadActivities = async () => {
    setIsLoading(true);
    const response = await profileService.getActivities(1, 20);
    if (response.success) {
      setActivities(response.data || []);
    }
    setIsLoading(false);
  };

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('vi-VN', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    }).format(date);
  };

  const getActionLabel = (action) => {
    const actionLabels = {
          'Login': 'Login',
      'Logout': 'Logout',
      'Register': 'Register',
      'Change Password': 'Change password',
      'Update Profile': 'Update profile',
      'Upload Avatar': 'Upload avatar',
      'Add Address': 'Add address',
      'Update Address': 'Update address',
      'Delete Address': 'Delete address',
      'Verify Email': 'Verify email',
      'Request Verification Email': 'Request verification email',
      'Create Booking': 'Create booking',
      'Cancel Booking': 'Cancel booking',
    };
    return actionLabels[action] || action;
  };

  if (isLoading) {
    return (
      <div className="flex justify-center py-8">
        <Loader2 className="w-8 h-8 animate-spin text-teal-600" />
      </div>
    );
  }

  return (
    <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
      <div className="flex items-center gap-3 mb-6">
        <div className="w-10 h-10 rounded-lg bg-cyan-100 flex items-center justify-center">
          <Activity className="w-5 h-5 text-cyan-600" />
        </div>
        <div>
          <h2 className="text-lg font-semibold text-gray-900">Activity history</h2>
          <p className="text-sm text-gray-500">View your recent activities</p>
        </div>
      </div>

      {activities.length === 0 ? (
        <div className="text-center py-8 text-gray-500">
          <Activity className="w-12 h-12 mx-auto mb-3 text-gray-300" />
          <p>No activities yet</p>
        </div>
      ) : (
        <div className="space-y-4">
          {activities.map((activity) => (
            <div key={activity.id} className="flex items-start gap-4 p-4 bg-gray-50 rounded-lg">
              <div className="w-10 h-10 rounded-full bg-teal-100 flex items-center justify-center flex-shrink-0">
                <Activity className="w-5 h-5 text-teal-600" />
              </div>
              <div className="flex-1 min-w-0">
                <p className="font-medium text-gray-900">{getActionLabel(activity.action)}</p>
                {activity.description && (
                  <p className="text-sm text-gray-500 mt-1">{activity.description}</p>
                )}
                <div className="flex items-center gap-4 mt-2 text-xs text-gray-400">
                  <span className="flex items-center gap-1">
                    <Clock className="w-3 h-3" />
                    {formatDate(activity.timestamp)}
                  </span>
                  {activity.ipAddress && (
                    <span className="flex items-center gap-1">
                      <Monitor className="w-3 h-3" />
                      {activity.ipAddress}
                    </span>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default ActivityLog;
