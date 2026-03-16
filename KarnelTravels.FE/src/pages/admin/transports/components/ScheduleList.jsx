import { Edit, Trash2, Plus, Calendar, Clock, DollarSign, MapPin } from 'lucide-react';

const ScheduleList = ({ schedules, vehicles, routes, isLoading, onCreate, onEdit, onDelete }) => {
  const formatTime = (time) => {
    if (!time) return '-';
    const [hours, minutes] = time.split(':');
    return `${hours.padStart(2, '0')}:${minutes.padStart(2, '0')}`;
  };

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
  };

  const getStatusBadge = (status) => {
    const statusConfig = {
      Active: { label: 'Hoạt động', className: 'bg-green-100 text-green-800' },
      Inactive: { label: 'Không hoạt động', className: 'bg-gray-100 text-gray-800' },
      Cancelled: { label: 'Cancelled', className: 'bg-red-100 text-red-800' },
    };
    const config = statusConfig[status] || { label: status, className: 'bg-gray-100 text-gray-800' };
    return (
      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${config.className}`}>
        {config.label}
      </span>
    );
  };

  if (isLoading) {
    return (
      <div className="p-8 text-center">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-teal-500 mx-auto"></div>
        <p className="mt-2 text-gray-500">Đang tải...</p>
      </div>
    );
  }

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-lg font-semibold text-gray-900">Danh sách lịch trình</h2>
        <button
          onClick={onCreate}
          className="inline-flex items-center px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-colors"
        >
          <Plus className="w-4 h-4 mr-2" />
          Thêm lịch trình
        </button>
      </div>

      {schedules.length === 0 ? (
        <div className="text-center py-12">
          <Calendar className="w-12 h-12 text-gray-300 mx-auto mb-4" />
          <p className="text-gray-500">Chưa có lịch trình nào</p>
        </div>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-200">
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Tuyến đường</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Phương tiện</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Giờ khởi hành</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Giờ đến</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Giá vé</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Ghế trống/Tổng</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Trạng thái</th>
                <th className="text-right py-3 px-4 text-sm font-medium text-gray-500">Thao tác</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {schedules.map((schedule) => (
                <tr key={schedule.scheduleId} className="hover:bg-gray-50">
                  <td className="py-3 px-4">
                    <div className="flex items-center gap-2">
                      <MapPin className="w-4 h-4 text-green-500" />
                      <span className="text-gray-900">{schedule.departureLocation}</span>
                      <span className="text-gray-400">→</span>
                      <span className="text-gray-900">{schedule.arrivalLocation}</span>
                    </div>
                  </td>
                  <td className="py-3 px-4 text-gray-600">
                    <div>
                      <span className="font-medium">{schedule.vehicleName}</span>
                      <span className="text-gray-400 text-sm ml-2">({schedule.vehicleLicensePlate})</span>
                    </div>
                  </td>
                  <td className="py-3 px-4">
                    <div className="flex items-center gap-2 text-gray-600">
                      <Clock className="w-4 h-4" />
                      {formatTime(schedule.departureTime)}
                    </div>
                  </td>
                  <td className="py-3 px-4 text-gray-600">
                    {schedule.arrivalTime ? formatTime(schedule.arrivalTime) : '-'}
                  </td>
                  <td className="py-3 px-4">
                    <span className="font-medium text-teal-600">{formatCurrency(schedule.price)}</span>
                  </td>
                  <td className="py-3 px-4 text-gray-600">
                    {schedule.availableSeats} / {schedule.totalSeats}
                  </td>
                  <td className="py-3 px-4">
                    {getStatusBadge(schedule.status)}
                  </td>
                  <td className="py-3 px-4 text-right">
                    <div className="flex items-center justify-end gap-2">
                      <button
                        onClick={() => onEdit(schedule)}
                        className="p-2 text-gray-400 hover:text-teal-600 hover:bg-teal-50 rounded-lg transition-colors"
                      >
                        <Edit className="w-4 h-4" />
                      </button>
                      <button
                        onClick={() => onDelete(schedule.scheduleId)}
                        className="p-2 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                      >
                        <Trash2 className="w-4 h-4" />
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default ScheduleList;
