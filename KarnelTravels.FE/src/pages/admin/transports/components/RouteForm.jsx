import { useState, useEffect } from 'react';
import { X } from 'lucide-react';

const RouteForm = ({ isOpen, onClose, onSave, mode, data }) => {
  const [formData, setFormData] = useState({
    departureLocation: data?.departureLocation || '',
    arrivalLocation: data?.arrivalLocation || '',
    routeName: data?.routeName || '',
    distance: data?.distance || '',
    description: data?.description || '',
    estimatedDuration: data?.estimatedDuration || ''
  });

  useEffect(() => {
    if (data) {
      setFormData({
        departureLocation: data.departureLocation || '',
        arrivalLocation: data.arrivalLocation || '',
        routeName: data.routeName || '',
        distance: data.distance || '',
        description: data.description || '',
        estimatedDuration: data.estimatedDuration || ''
      });
    }
  }, [data]);

  if (!isOpen) return null;

  const handleSubmit = (e) => {
    e.preventDefault();
    const submitData = {
      ...formData,
      distance: formData.distance ? parseFloat(formData.distance) : null
    };
    onSave(submitData);
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
      <div className="bg-white rounded-xl shadow-xl w-full max-w-2xl max-h-[90vh] overflow-y-auto">
        <div className="flex items-center justify-between p-6 border-b border-gray-200">
          <h2 className="text-xl font-semibold text-gray-900">
            {mode === 'create' ? 'Thêm tuyến đường mới' : 'Cập nhật tuyến đường'}
          </h2>
          <button onClick={onClose} className="p-2 text-gray-400 hover:text-gray-600 rounded-lg hover:bg-gray-100">
            <X className="w-5 h-5" />
          </button>
        </div>
        <form onSubmit={handleSubmit} className="p-6 space-y-4">
          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Điểm đi *</label>
              <input
                type="text"
                required
                value={formData.departureLocation}
                onChange={(e) => setFormData({ ...formData, departureLocation: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500"
                placeholder="Ví dụ: Hà Nội"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Điểm đến *</label>
              <input
                type="text"
                required
                value={formData.arrivalLocation}
                onChange={(e) => setFormData({ ...formData, arrivalLocation: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500"
                placeholder="Ví dụ: TP. Hồ Chí Minh"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Tên tuyến</label>
            <input
              type="text"
              value={formData.routeName}
              onChange={(e) => setFormData({ ...formData, routeName: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500"
              placeholder="Hà Nội - TP. Hồ Chí Minh"
            />
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Khoảng cách (km)</label>
              <input
                type="number"
                step="0.1"
                value={formData.distance}
                onChange={(e) => setFormData({ ...formData, distance: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500"
              />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Thời gian ước tính</label>
              <input
                type="text"
                value={formData.estimatedDuration}
                onChange={(e) => setFormData({ ...formData, estimatedDuration: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500"
                placeholder="Ví dụ: 2 giờ 30 phút"
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Mô tả</label>
            <textarea
              rows={3}
              value={formData.description}
              onChange={(e) => setFormData({ ...formData, description: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500"
            />
          </div>

          <div className="flex justify-end gap-3 pt-4">
            <button
              type="button"
              onClick={onClose}
              className="px-4 py-2 text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
            >
              Hủy
            </button>
            <button
              type="submit"
              className="px-4 py-2 bg-teal-500 text-white rounded-lg hover:bg-teal-600 transition-colors"
            >
              {mode === 'create' ? 'Thêm mới' : 'Lưu thay đổi'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default RouteForm;
