import { Edit, Trash2, Eye, Plus, Bus } from 'lucide-react';

const TransportList = ({
  vehicles,
  vehicleTypes,
  providers,
  isLoading,
  onCreate,
  onEdit,
  onDelete
}) => {
  const getStatusBadge = (status) => {
    const statusConfig = {
      Available: { label: 'Hoạt động', className: 'bg-green-100 text-green-800' },
      InService: { label: 'Đang chạy', className: 'bg-blue-100 text-blue-800' },
      Maintenance: { label: 'Bảo trì', className: 'bg-yellow-100 text-yellow-800' },
      Retired: { label: 'Ngưng hoạt động', className: 'bg-gray-100 text-gray-800' },
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
        <h2 className="text-lg font-semibold text-gray-900">Danh sách phương tiện</h2>
        <button
          onClick={onCreate}
          className="inline-flex items-center px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-colors"
        >
          <Plus className="w-4 h-4 mr-2" />
          Thêm phương tiện
        </button>
      </div>

      {vehicles.length === 0 ? (
        <div className="text-center py-12">
          <Bus className="w-12 h-12 text-gray-300 mx-auto mb-4" />
          <p className="text-gray-500">Chưa có phương tiện nào</p>
        </div>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-200">
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Tên phương tiện</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Biển số</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Loại</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Nhà vận chuyển</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Số ghế</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Trạng thái</th>
                <th className="text-right py-3 px-4 text-sm font-medium text-gray-500">Thao tác</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {vehicles.map((vehicle) => (
                <tr key={vehicle.vehicleId} className="hover:bg-gray-50">
                  <td className="py-3 px-4">
                    <div className="flex items-center gap-3">
                      {vehicle.imageUrl ? (
                        <img src={vehicle.imageUrl} alt={vehicle.name} className="w-10 h-10 rounded-lg object-cover" />
                      ) : (
                        <div className="w-10 h-10 rounded-lg bg-gray-100 flex items-center justify-center">
                          <Bus className="w-5 h-5 text-gray-400" />
                        </div>
                      )}
                      <span className="font-medium text-gray-900">{vehicle.name}</span>
                    </div>
                  </td>
                  <td className="py-3 px-4 text-gray-600">{vehicle.licensePlate}</td>
                  <td className="py-3 px-4 text-gray-600">{vehicle.vehicleTypeName}</td>
                  <td className="py-3 px-4 text-gray-600">{vehicle.providerName}</td>
                  <td className="py-3 px-4 text-gray-600">{vehicle.capacity}</td>
                  <td className="py-3 px-4">{getStatusBadge(vehicle.status)}</td>
                  <td className="py-3 px-4 text-right">
                    <div className="flex items-center justify-end gap-2">
                      <button
                        onClick={() => onEdit(vehicle)}
                        className="p-2 text-gray-400 hover:text-teal-600 hover:bg-teal-50 rounded-lg transition-colors"
                      >
                        <Edit className="w-4 h-4" />
                      </button>
                      <button
                        onClick={() => onDelete(vehicle.vehicleId)}
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

export default TransportList;
