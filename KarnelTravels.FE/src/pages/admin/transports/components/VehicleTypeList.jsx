import { Edit, Trash2, Plus, Bus } from 'lucide-react';

const VehicleTypeList = ({ vehicleTypes, isLoading, onCreate, onEdit, onDelete }) => {
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
        <h2 className="text-lg font-semibold text-gray-900">Danh sách loại phương tiện</h2>
        <button
          onClick={onCreate}
          className="inline-flex items-center px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-colors"
        >
          <Plus className="w-4 h-4 mr-2" />
          Thêm loại phương tiện
        </button>
      </div>

      {vehicleTypes.length === 0 ? (
        <div className="text-center py-12">
          <Bus className="w-12 h-12 text-gray-300 mx-auto mb-4" />
          <p className="text-gray-500">Chưa có loại phương tiện nào</p>
        </div>
      ) : (
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead>
              <tr className="border-b border-gray-200">
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Tên loại phương tiện</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Mô tả</th>
                <th className="text-left py-3 px-4 text-sm font-medium text-gray-500">Icon</th>
                <th className="text-right py-3 px-4 text-sm font-medium text-gray-500">Thao tác</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {vehicleTypes.map((type) => (
                <tr key={type.vehicleTypeId} className="hover:bg-gray-50">
                  <td className="py-3 px-4">
                    <div className="flex items-center gap-3">
                      <div className="w-10 h-10 rounded-lg bg-gray-100 flex items-center justify-center">
                        <Bus className="w-5 h-5 text-gray-400" />
                      </div>
                      <span className="font-medium text-gray-900">{type.name}</span>
                    </div>
                  </td>
                  <td className="py-3 px-4 text-gray-600">{type.description || '-'}</td>
                  <td className="py-3 px-4 text-gray-600">{type.icon || '-'}</td>
                  <td className="py-3 px-4 text-right">
                    <div className="flex items-center justify-end gap-2">
                      <button
                        onClick={() => onEdit(type)}
                        className="p-2 text-gray-400 hover:text-teal-600 hover:bg-teal-50 rounded-lg transition-colors"
                      >
                        <Edit className="w-4 h-4" />
                      </button>
                      <button
                        onClick={() => onDelete(type.vehicleTypeId)}
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

export default VehicleTypeList;
