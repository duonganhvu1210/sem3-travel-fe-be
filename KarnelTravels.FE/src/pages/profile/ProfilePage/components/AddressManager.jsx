import { useState, useEffect } from 'react';
import { profileService } from '@/services/profileService';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent } from '@/components/ui/card';
import { Loader2, Plus, Pencil, Trash2, MapPin, Check, X } from 'lucide-react';

const AddressManager = () => {
  const [addresses, setAddresses] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isEditing, setIsEditing] = useState(false);
  const [editingAddress, setEditingAddress] = useState(null);
  const [isSaving, setIsSaving] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' });
  const [formData, setFormData] = useState({
    addressLine: '',
    ward: '',
    district: '',
    city: '',
    country: 'Vietnam',
    isDefault: false,
  });

  useEffect(() => {
    loadAddresses();
  }, []);

  const loadAddresses = async () => {
    setIsLoading(true);
    const response = await profileService.getAddresses();
    if (response.success) {
      setAddresses(response.data || []);
    }
    setIsLoading(false);
  };

  const handleOpenModal = (address = null) => {
    if (address) {
      setEditingAddress(address);
      setFormData({
        addressLine: address.addressLine,
        ward: address.ward || '',
        district: address.district || '',
        city: address.city,
        country: address.country || 'Vietnam',
        isDefault: address.isDefault,
      });
    } else {
      setEditingAddress(null);
      setFormData({
        addressLine: '',
        ward: '',
        district: '',
        city: '',
        country: 'Vietnam',
        isDefault: addresses.length === 0,
      });
    }
    setIsEditing(true);
    setMessage({ type: '', text: '' });
  };

  const handleCloseModal = () => {
    setIsEditing(false);
    setEditingAddress(null);
    setMessage({ type: '', text: '' });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsSaving(true);
    setMessage({ type: '', text: '' });

    let response;
    if (editingAddress) {
      response = await profileService.updateAddress({
        addressId: editingAddress.addressId,
        ...formData,
      });
    } else {
      response = await profileService.createAddress(formData);
    }

    if (response.success) {
      setMessage({ type: 'success', text: editingAddress ? 'Cập nhật địa chỉ thành công!' : 'Thêm địa chỉ thành công!' });
      await loadAddresses();
      setTimeout(handleCloseModal, 1000);
    } else {
      setMessage({ type: 'error', text: response.message || 'Thao tác thất bại' });
    }

    setIsSaving(false);
  };

  const handleDelete = async (addressId) => {
    if (!window.confirm('Bạn có chắc chắn muốn xóa địa chỉ này?')) return;

    const response = await profileService.deleteAddress(addressId);
    if (response.success) {
      await loadAddresses();
    } else {
      alert(response.message || 'Xóa thất bại');
    }
  };

  const handleSetDefault = async (addressId) => {
    const response = await profileService.setDefaultAddress(addressId);
    if (response.success) {
      await loadAddresses();
    }
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
      <div className="flex items-center justify-between mb-6">
        <h2 className="text-lg font-semibold text-gray-900">Địa chỉ của tôi</h2>
        <Button onClick={() => handleOpenModal()} className="bg-teal-600 hover:bg-teal-700">
          <Plus className="w-4 h-4 mr-2" />
          Thêm địa chỉ
        </Button>
      </div>

      {addresses.length === 0 ? (
        <div className="text-center py-8 text-gray-500">
          <MapPin className="w-12 h-12 mx-auto mb-3 text-gray-300" />
          <p>No address yet</p>
          <p className="text-sm">Thêm địa chỉ để sử dụng khi đặt tour</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {addresses.map((address) => (
            <Card key={address.addressId} className={`${address.isDefault ? 'border-teal-500 bg-teal-50' : ''}`}>
              <CardContent className="p-4">
                <div className="flex items-start justify-between">
                  <div className="flex-1">
                    {address.isDefault && (
                      <span className="inline-flex items-center px-2 py-1 rounded-full text-xs font-medium bg-teal-100 text-teal-800 mb-2">
                        <Check className="w-3 h-3 mr-1" />
                        Mặc định
                      </span>
                    )}
                    <p className="font-medium text-gray-900">{address.addressLine}</p>
                    <p className="text-sm text-gray-500">
                      {[address.ward, address.district, address.city, address.country].filter(Boolean).join(', ')}
                    </p>
                  </div>
                  <div className="flex gap-1">
                    {!address.isDefault && (
                      <Button variant="ghost" size="sm" onClick={() => handleSetDefault(address.addressId)} title="Đặt làm mặc định">
                        <Check className="w-4 h-4 text-gray-400 hover:text-teal-600" />
                      </Button>
                    )}
                    <Button variant="ghost" size="sm" onClick={() => handleOpenModal(address)} title="Chỉnh sửa">
                      <Pencil className="w-4 h-4 text-gray-400 hover:text-teal-600" />
                    </Button>
                    <Button variant="ghost" size="sm" onClick={() => handleDelete(address.addressId)} title="Xóa">
                      <Trash2 className="w-4 h-4 text-gray-400 hover:text-red-600" />
                    </Button>
                  </div>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {/* Modal */}
      {isEditing && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl max-w-md w-full p-6 max-h-[90vh] overflow-y-auto">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-semibold">{editingAddress ? 'Chỉnh sửa địa chỉ' : 'Thêm địa chỉ mới'}</h3>
              <Button variant="ghost" size="sm" onClick={handleCloseModal}>
                <X className="w-5 h-5" />
              </Button>
            </div>

            {message.text && (
              <div className={`mb-4 p-3 rounded-lg text-sm ${message.type === 'success' ? 'bg-green-50 text-green-700' : 'bg-red-50 text-red-700'}`}>
                {message.text}
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="addressLine">Địa chỉ</Label>
                <Input
                  id="addressLine"
                  value={formData.addressLine}
                  onChange={(e) => setFormData({ ...formData, addressLine: e.target.value })}
                  placeholder="Số nhà, đường..."
                  required
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="ward">Phường/Xã</Label>
                  <Input
                    id="ward"
                    value={formData.ward}
                    onChange={(e) => setFormData({ ...formData, ward: e.target.value })}
                    placeholder="Phường/Xã"
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="district">Quận/Huyện</Label>
                  <Input
                    id="district"
                    value={formData.district}
                    onChange={(e) => setFormData({ ...formData, district: e.target.value })}
                    placeholder="Quận/Huyện"
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                  <Label htmlFor="city">Thành phố/Tỉnh</Label>
                  <Input
                    id="city"
                    value={formData.city}
                    onChange={(e) => setFormData({ ...formData, city: e.target.value })}
                    placeholder="Thành phố/Tỉnh"
                    required
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="country">Quốc gia</Label>
                  <Input
                    id="country"
                    value={formData.country}
                    onChange={(e) => setFormData({ ...formData, country: e.target.value })}
                  />
                </div>
              </div>

              <div className="flex items-center gap-2">
                <input
                  type="checkbox"
                  id="isDefault"
                  checked={formData.isDefault}
                  onChange={(e) => setFormData({ ...formData, isDefault: e.target.checked })}
                  className="w-4 h-4 text-teal-600 rounded"
                />
                <Label htmlFor="isDefault" className="cursor-pointer">Đặt làm địa chỉ mặc định</Label>
              </div>

              <div className="flex gap-3 pt-4">
                <Button type="submit" disabled={isSaving} className="bg-teal-600 hover:bg-teal-700">
                  {isSaving ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : null}
                  {editingAddress ? 'Cập nhật' : 'Thêm mới'}
                </Button>
                <Button type="button" variant="outline" onClick={handleCloseModal}>
                  Hủy
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default AddressManager;
