import { useState, useEffect, useCallback } from 'react';
import { Plus, Bus, Route as RouteIcon, Calendar, Building2, Edit, Trash2, Eye, X, Check, XCircle } from 'lucide-react';
import toast from 'react-hot-toast';
import {
  vehicleService,
  vehicleTypeService,
  transportProviderService,
  routeService,
  scheduleService
} from '@/services/vehicleService';
import TransportList from './components/TransportList';
import TransportForm from './components/TransportForm';
import ProviderList from './components/ProviderList';
import ProviderForm from './components/ProviderForm';
import RouteList from './components/RouteList';
import RouteForm from './components/RouteForm';
import ScheduleList from './components/ScheduleList';
import ScheduleForm from './components/ScheduleForm';
import VehicleTypeList from './components/VehicleTypeList';
import VehicleTypeForm from './components/VehicleTypeForm';

const TransportsPage = () => {
  // Active tab: 'vehicles', 'providers', 'routes', 'schedules', 'types'
  const [activeTab, setActiveTab] = useState('vehicles');

  // Data states
  const [vehicles, setVehicles] = useState([]);
  const [providers, setProviders] = useState([]);
  const [routes, setRoutes] = useState([]);
  const [schedules, setSchedules] = useState([]);
  const [vehicleTypes, setVehicleTypes] = useState([]);
  const [isLoading, setIsLoading] = useState(true);

  // Form modal states
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [formMode, setFormMode] = useState('create');
  const [selectedItem, setSelectedItem] = useState(null);

  // Load data on tab change
  useEffect(() => {
    loadData();
  }, [activeTab]);

  const loadData = async () => {
    setIsLoading(true);
    try {
      switch (activeTab) {
        case 'vehicles':
          const vehiclesRes = await vehicleService.getAll();
          if (vehiclesRes.success) setVehicles(vehiclesRes.data || []);
          const typesRes = await vehicleTypeService.getAll();
          if (typesRes.success) setVehicleTypes(typesRes.data || []);
          const providersRes = await transportProviderService.getAll();
          if (providersRes.success) setProviders(providersRes.data || []);
          const routesRes = await routeService.getAll();
          if (routesRes.success) setRoutes(routesRes.data || []);
          break;
        case 'providers':
          const pRes = await transportProviderService.getAll();
          if (pRes.success) setProviders(pRes.data || []);
          break;
        case 'routes':
          const rRes = await routeService.getAll();
          if (rRes.success) setRoutes(rRes.data || []);
          break;
        case 'schedules':
          const sRes = await scheduleService.getAll();
          if (sRes.success) setSchedules(sRes.data || []);
          const vRes = await vehicleService.getAll();
          if (vRes.success) setVehicles(vRes.data || []);
          const rtRes = await routeService.getAll();
          if (rtRes.success) setRoutes(rtRes.data || []);
          break;
        case 'types':
          const vtRes = await vehicleTypeService.getAll();
          if (vtRes.success) setVehicleTypes(vtRes.data || []);
          break;
        default:
          break;
      }
    } catch (error) {
      console.error('Error loading data:', error);
      toast.error('Không thể tải dữ liệu');
    } finally {
      setIsLoading(false);
    }
  };

  // Handlers for vehicles
  const handleCreateVehicle = () => {
    setFormMode('create');
    setSelectedItem(null);
    setIsFormOpen(true);
  };

  const handleEditVehicle = (vehicle) => {
    setFormMode('edit');
    setSelectedItem(vehicle);
    setIsFormOpen(true);
  };

  const handleDeleteVehicle = async (id) => {
    if (!window.confirm('Bạn có chắc muốn xóa phương tiện này?')) return;
    try {
      const res = await vehicleService.delete(id);
      if (res.success) {
        toast.success('Xóa phương tiện thành công');
        loadData();
      } else {
        toast.error(res.message || 'Xóa thất bại');
      }
    } catch (error) {
      toast.error('Lỗi khi xóa phương tiện');
    }
  };

  const handleSaveVehicle = async (data) => {
    try {
      let res;
      if (formMode === 'create') {
        res = await vehicleService.create(data);
      } else {
        res = await vehicleService.update(selectedItem.vehicleId, data);
      }
      if (res.success) {
        toast.success(formMode === 'create' ? 'Thêm phương tiện thành công' : 'Cập nhật thành công');
        setIsFormOpen(false);
        loadData();
      } else {
        toast.error(res.message || 'Thao tác thất bại');
      }
    } catch (error) {
      toast.error('Lỗi khi lưu phương tiện');
    }
  };

  // Handlers for providers
  const handleCreateProvider = () => {
    setFormMode('create');
    setSelectedItem(null);
    setIsFormOpen(true);
  };

  const handleEditProvider = (provider) => {
    setFormMode('edit');
    setSelectedItem(provider);
    setIsFormOpen(true);
  };

  const handleDeleteProvider = async (id) => {
    if (!window.confirm('Bạn có chắc muốn xóa nhà vận chuyển này?')) return;
    try {
      const res = await transportProviderService.delete(id);
      if (res.success) {
        toast.success('Xóa thành công');
        loadData();
      } else {
        toast.error(res.message || 'Xóa thất bại');
      }
    } catch (error) {
      toast.error('Lỗi khi xóa');
    }
  };

  const handleSaveProvider = async (data) => {
    try {
      let res;
      if (formMode === 'create') {
        res = await transportProviderService.create(data);
      } else {
        res = await transportProviderService.update(selectedItem.providerId, data);
      }
      if (res.success) {
        toast.success(formMode === 'create' ? 'Thêm thành công' : 'Cập nhật thành công');
        setIsFormOpen(false);
        loadData();
      } else {
        toast.error(res.message || 'Thao tác thất bại');
      }
    } catch (error) {
      toast.error('Lỗi khi lưu');
    }
  };

  // Handlers for routes
  const handleCreateRoute = () => {
    setFormMode('create');
    setSelectedItem(null);
    setIsFormOpen(true);
  };

  const handleEditRoute = (route) => {
    setFormMode('edit');
    setSelectedItem(route);
    setIsFormOpen(true);
  };

  const handleDeleteRoute = async (id) => {
    if (!window.confirm('Bạn có chắc muốn xóa tuyến đường này?')) return;
    try {
      const res = await routeService.delete(id);
      if (res.success) {
        toast.success('Xóa thành công');
        loadData();
      } else {
        toast.error(res.message || 'Xóa thất bại');
      }
    } catch (error) {
      toast.error('Lỗi khi xóa');
    }
  };

  const handleSaveRoute = async (data) => {
    try {
      let res;
      if (formMode === 'create') {
        res = await routeService.create(data);
      } else {
        res = await routeService.update(selectedItem.routeId, data);
      }
      if (res.success) {
        toast.success(formMode === 'create' ? 'Thêm thành công' : 'Cập nhật thành công');
        setIsFormOpen(false);
        loadData();
      } else {
        toast.error(res.message || 'Thao tác thất bại');
      }
    } catch (error) {
      toast.error('Lỗi khi lưu');
    }
  };

  // Handlers for schedules
  const handleCreateSchedule = () => {
    setFormMode('create');
    setSelectedItem(null);
    setIsFormOpen(true);
  };

  const handleEditSchedule = (schedule) => {
    setFormMode('edit');
    setSelectedItem(schedule);
    setIsFormOpen(true);
  };

  const handleDeleteSchedule = async (id) => {
    if (!window.confirm('Bạn có chắc muốn xóa lịch trình này?')) return;
    try {
      const res = await scheduleService.delete(id);
      if (res.success) {
        toast.success('Xóa thành công');
        loadData();
      } else {
        toast.error(res.message || 'Xóa thất bại');
      }
    } catch (error) {
      toast.error('Lỗi khi xóa');
    }
  };

  const handleSaveSchedule = async (data) => {
    try {
      let res;
      if (formMode === 'create') {
        res = await scheduleService.create(data);
      } else {
        res = await scheduleService.update(selectedItem.scheduleId, data);
      }
      if (res.success) {
        toast.success(formMode === 'create' ? 'Thêm thành công' : 'Cập nhật thành công');
        setIsFormOpen(false);
        loadData();
      } else {
        toast.error(res.message || 'Thao tác thất bại');
      }
    } catch (error) {
      toast.error('Lỗi khi lưu');
    }
  };

  // Handlers for vehicle types
  const handleCreateVehicleType = () => {
    setFormMode('create');
    setSelectedItem(null);
    setIsFormOpen(true);
  };

  const handleEditVehicleType = (type) => {
    setFormMode('edit');
    setSelectedItem(type);
    setIsFormOpen(true);
  };

  const handleDeleteVehicleType = async (id) => {
    if (!window.confirm('Bạn có chắc muốn xóa loại phương tiện này?')) return;
    try {
      const res = await vehicleTypeService.delete(id);
      if (res.success) {
        toast.success('Xóa thành công');
        loadData();
      } else {
        toast.error(res.message || 'Xóa thất bại');
      }
    } catch (error) {
      toast.error('Lỗi khi xóa');
    }
  };

  const handleSaveVehicleType = async (data) => {
    try {
      let res;
      if (formMode === 'create') {
        res = await vehicleTypeService.create(data);
      } else {
        res = await vehicleTypeService.update(selectedItem.vehicleTypeId, data);
      }
      if (res.success) {
        toast.success(formMode === 'create' ? 'Thêm thành công' : 'Cập nhật thành công');
        setIsFormOpen(false);
        loadData();
      } else {
        toast.error(res.message || 'Thao tác thất bại');
      }
    } catch (error) {
      toast.error('Lỗi khi lưu');
    }
  };

  const tabs = [
    { id: 'vehicles', label: 'Phương tiện', icon: Bus },
    { id: 'providers', label: 'Nhà vận chuyển', icon: Building2 },
    { id: 'routes', label: 'Tuyến đường', icon: RouteIcon },
    { id: 'schedules', label: 'Lịch trình', icon: Calendar },
    { id: 'types', label: 'Loại phương tiện', icon: Bus },
  ];

  return (
    <div className="space-y-6">
      {/* Page Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Quản lý Phương tiện</h1>
          <p className="text-sm text-gray-500 mt-1">Quản lý phương tiện vận chuyển, nhà xe, tuyến đường và lịch trình</p>
        </div>
      </div>

      {/* Tabs */}
      <div className="border-b border-gray-200">
        <nav className="-mb-px flex space-x-8">
          {tabs.map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`
                group inline-flex items-center py-4 px-1 border-b-2 font-medium text-sm transition-colors
                ${activeTab === tab.id
                  ? 'border-teal-500 text-teal-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
                }
              `}
            >
              <tab.icon className={`w-5 h-5 mr-2 ${activeTab === tab.id ? 'text-teal-500' : 'text-gray-400 group-hover:text-gray-500'}`} />
              {tab.label}
            </button>
          ))}
        </nav>
      </div>

      {/* Content */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-200">
        {activeTab === 'vehicles' && (
          <TransportList
            vehicles={vehicles}
            vehicleTypes={vehicleTypes}
            providers={providers}
            isLoading={isLoading}
            onCreate={handleCreateVehicle}
            onEdit={handleEditVehicle}
            onDelete={handleDeleteVehicle}
          />
        )}
        {activeTab === 'providers' && (
          <ProviderList
            providers={providers}
            isLoading={isLoading}
            onCreate={handleCreateProvider}
            onEdit={handleEditProvider}
            onDelete={handleDeleteProvider}
          />
        )}
        {activeTab === 'routes' && (
          <RouteList
            routes={routes}
            isLoading={isLoading}
            onCreate={handleCreateRoute}
            onEdit={handleEditRoute}
            onDelete={handleDeleteRoute}
          />
        )}
        {activeTab === 'schedules' && (
          <ScheduleList
            schedules={schedules}
            vehicles={vehicles}
            routes={routes}
            isLoading={isLoading}
            onCreate={handleCreateSchedule}
            onEdit={handleEditSchedule}
            onDelete={handleDeleteSchedule}
          />
        )}
        {activeTab === 'types' && (
          <VehicleTypeList
            vehicleTypes={vehicleTypes}
            isLoading={isLoading}
            onCreate={handleCreateVehicleType}
            onEdit={handleEditVehicleType}
            onDelete={handleDeleteVehicleType}
          />
        )}
      </div>

      {/* Modals */}
      {isFormOpen && activeTab === 'vehicles' && (
        <TransportForm
          isOpen={isFormOpen}
          onClose={() => setIsFormOpen(false)}
          onSave={handleSaveVehicle}
          mode={formMode}
          data={selectedItem}
          vehicleTypes={vehicleTypes}
          providers={providers}
        />
      )}
      {isFormOpen && activeTab === 'providers' && (
        <ProviderForm
          isOpen={isFormOpen}
          onClose={() => setIsFormOpen(false)}
          onSave={handleSaveProvider}
          mode={formMode}
          data={selectedItem}
        />
      )}
      {isFormOpen && activeTab === 'routes' && (
        <RouteForm
          isOpen={isFormOpen}
          onClose={() => setIsFormOpen(false)}
          onSave={handleSaveRoute}
          mode={formMode}
          data={selectedItem}
        />
      )}
      {isFormOpen && activeTab === 'schedules' && (
        <ScheduleForm
          isOpen={isFormOpen}
          onClose={() => setIsFormOpen(false)}
          onSave={handleSaveSchedule}
          mode={formMode}
          data={selectedItem}
          vehicles={vehicles}
          routes={routes}
        />
      )}
      {isFormOpen && activeTab === 'types' && (
        <VehicleTypeForm
          isOpen={isFormOpen}
          onClose={() => setIsFormOpen(false)}
          onSave={handleSaveVehicleType}
          mode={formMode}
          data={selectedItem}
        />
      )}
    </div>
  );
};

export default TransportsPage;
