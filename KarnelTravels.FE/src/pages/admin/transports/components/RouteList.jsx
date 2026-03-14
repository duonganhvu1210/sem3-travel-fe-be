import { Edit, Trash2, Plus, MapPin, Route as RouteIcon } from 'lucide-react';

const RouteList = ({ routes, isLoading, onCreate, onEdit, onDelete }) => {
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
        <h2 className="text-lg font-semibold text-gray-900">Danh sách tuyến đường</h2>
        <button
          onClick={onCreate}
          className="inline-flex items-center px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-colors"
        >
          <Plus className="w-4 h-4 mr-2" />
          Thêm tuyến đường
        </button>
      </div>

      {routes.length === 0 ? (
        <div className="text-center py-12">
          <RouteIcon className="w-12 h-12 text-gray-300 mx-auto mb-4" />
          <p className="text-gray-500">Chưa có tuyến đường nào</p>
        </div>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-200">
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Tên tuyến</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Điểm đi</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Điểm đến</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Khoảng cách</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Thời gian ước tính</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Số lịch trình</th>
                <th className="text-right py-3 px-4 text-sm font-medium text-gray-500">Thao tác</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {routes.map((route) => (
                <tr key={route.routeId} className="hover:bg-gray-50">
                  <td className="py-3 px-4 font-medium text-gray-900">{route.routeName || '-'}</td>
                  <td className="py-3 px-4 text-gray-600">
                    <div className="flex items-center gap-2">
                      <MapPin className="w-4 h-4 text-green-500" />
                      {route.departureLocation}
                    </div>
                  </td>
                  <td className="py-3 px-4 text-gray-600">
                    <div className="flex items-center gap-2">
                      <MapPin className="w-4 h-4 text-red-500" />
                      {route.arrivalLocation}
                    </div>
                  </td>
                  <td className="py-3 px-4 text-gray-600">{route.distance ? `${route.distance} km` : '-'}</td>
                  <td className="py-3 px-4 text-gray-600">{route.estimatedDuration || '-'}</td>
                  <td className="py-3 px-4 text-gray-600">{route.scheduleCount}</td>
                  <td className="py-3 px-4 text-right">
                    <div className="flex items-center justify-end gap-2">
                      <button
                        onClick={() => onEdit(route)}
                        className="p-2 text-gray-400 hover:text-teal-600 hover:bg-teal-50 rounded-lg transition-colors"
                      >
                        <Edit className="w-4 h-4" />
                      </button>
                      <button
                        onClick={() => onDelete(route.routeId)}
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

export default RouteList;
