import { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog';
import { toast } from 'sonner';
import { Plus, Search, Users, Filter } from 'lucide-react';
import UserList from './components/UserList';
import UserForm from './components/UserForm';
import ChangePasswordModal from './components/ChangePasswordModal';
import UserHistory from './components/UserHistory';
import userService from '@/services/userService';

export default function UsersManagement() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [pagination, setPagination] = useState({
    pageNumber: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0,
    hasPreviousPage: false,
    hasNextPage: false,
  });

  // Search and filter states
  const [searchTerm, setSearchTerm] = useState('');
  const [roleFilter, setRoleFilter] = useState('');
  const [statusFilter, setStatusFilter] = useState('');

  // Modal states
  const [userFormOpen, setUserFormOpen] = useState(false);
  const [passwordModalOpen, setPasswordModalOpen] = useState(false);
  const [historyModalOpen, setHistoryModalOpen] = useState(false);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [statusDialogOpen, setStatusDialogOpen] = useState(false);

  // Selected user for actions
  const [selectedUser, setSelectedUser] = useState(null);
  const [formLoading, setFormLoading] = useState(false);

  useEffect(() => {
    fetchUsers();
  }, [pagination.pageNumber, roleFilter, statusFilter]);

  const fetchUsers = async () => {
    setLoading(true);
    try {
      const params = {
        pageNumber: pagination.pageNumber,
        pageSize: pagination.pageSize,
        searchTerm: searchTerm || undefined,
        role: roleFilter ? roleFilter : undefined,
        isLocked: statusFilter ? statusFilter === 'locked' : undefined,
      };

      const response = await userService.getUsers(params);
      if (response.success) {
        setUsers(response.data.users);
        setPagination({
          ...pagination,
          totalCount: response.data.totalCount,
          totalPages: response.data.totalPages,
          hasPreviousPage: response.data.hasPreviousPage,
          hasNextPage: response.data.hasNextPage,
        });
      }
    } catch (error) {
      console.error('Error fetching users:', error);
      toast.error('Không thể tải danh sách người dùng');
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = () => {
    setPagination({ ...pagination, pageNumber: 1 });
    fetchUsers();
  };

  const handlePageChange = (newPage) => {
    setPagination({ ...pagination, pageNumber: newPage });
  };

  // Handlers
  const handleEdit = (user) => {
    setSelectedUser(user);
    setUserFormOpen(true);
  };

  const handleDelete = (user) => {
    setSelectedUser(user);
    setDeleteDialogOpen(true);
  };

  const handleToggleStatus = (user) => {
    setSelectedUser(user);
    setStatusDialogOpen(true);
  };

  const handleResetPassword = (user) => {
    setSelectedUser(user);
    setPasswordModalOpen(true);
  };

  const handleViewHistory = (user) => {
    setSelectedUser(user);
    setHistoryModalOpen(true);
  };

  // Submit handlers
  const handleCreateUser = async (data) => {
    setFormLoading(true);
    try {
      const response = await userService.createUser(data);
      if (response.success) {
        toast.success('Tạo người dùng thành công');
        setUserFormOpen(false);
        fetchUsers();
      } else {
        toast.error(response.message || 'Không thể tạo người dùng');
      }
    } catch (error) {
      console.error('Error creating user:', error);
      toast.error(error.response?.data?.message || 'Không thể tạo người dùng');
    } finally {
      setFormLoading(false);
    }
  };

  const handleUpdateUser = async (data) => {
    setFormLoading(true);
    try {
      const response = await userService.updateUser(selectedUser.userId, data);
      if (response.success) {
        toast.success('Cập nhật người dùng thành công');
        setUserFormOpen(false);
        fetchUsers();
      } else {
        toast.error(response.message || 'Không thể cập nhật người dùng');
      }
    } catch (error) {
      console.error('Error updating user:', error);
      toast.error(error.response?.data?.message || 'Không thể cập nhật người dùng');
    } finally {
      setFormLoading(false);
    }
  };

  const handleFormSubmit = (data) => {
    if (selectedUser) {
      handleUpdateUser(data);
    } else {
      handleCreateUser(data);
    }
  };

  const confirmDelete = async () => {
    try {
      const response = await userService.deleteUser(selectedUser.userId);
      if (response.success) {
        toast.success('Xóa người dùng thành công');
        fetchUsers();
      } else {
        toast.error(response.message || 'Không thể xóa người dùng');
      }
    } catch (error) {
      console.error('Error deleting user:', error);
      toast.error('Không thể xóa người dùng');
    } finally {
      setDeleteDialogOpen(false);
      setSelectedUser(null);
    }
  };

  const confirmToggleStatus = async () => {
    try {
      const response = await userService.updateUserStatus(
        selectedUser.userId,
        !selectedUser.isLocked
      );
      if (response.success) {
        toast.success(
          selectedUser.isLocked
            ? 'Kích hoạt tài khoản thành công'
            : 'Vô hiệu hóa tài khoản thành công'
        );
        fetchUsers();
      } else {
        toast.error(response.message || 'Không thể thay đổi trạng thái');
      }
    } catch (error) {
      console.error('Error toggling user status:', error);
      toast.error('Không thể thay đổi trạng thái');
    } finally {
      setStatusDialogOpen(false);
      setSelectedUser(null);
    }
  };

  const handlePasswordReset = async (newPassword) => {
    try {
      const response = await userService.resetPassword(selectedUser.userId, newPassword);
      if (response.success) {
        toast.success('Đặt lại mật khẩu thành công');
        setPasswordModalOpen(false);
      } else {
        toast.error(response.message || 'Không thể đặt lại mật khẩu');
      }
    } catch (error) {
      console.error('Error resetting password:', error);
      toast.error('Không thể đặt lại mật khẩu');
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Quản lý người dùng</h1>
          <p className="text-muted-foreground">
            Quản lý tài khoản và phân quyền người dùng
          </p>
        </div>
        <Button onClick={() => { setSelectedUser(null); setUserFormOpen(true); }}>
          <Plus className="w-4 h-4 mr-2" />
          Thêm người dùng
        </Button>
      </div>

      {/* Filters */}
      <Card>
        <CardHeader className="pb-4">
          <CardTitle className="text-lg flex items-center gap-2">
            <Filter className="w-4 h-4" />
            Bộ lọc tìm kiếm
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex flex-wrap gap-4">
            <div className="flex-1 min-w-[200px]">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                  placeholder="Tìm theo tên, email, số điện thoại..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                  className="pl-9"
                />
              </div>
            </div>
            <Select value={roleFilter} onValueChange={(value) => { setRoleFilter(value === 'all' ? '' : value); setPagination({ ...pagination, pageNumber: 1 }); }}>
              <SelectTrigger className="w-[180px]">
                <SelectValue placeholder="Lọc theo vai trò" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Tất cả vai trò</SelectItem>
                <SelectItem value="Admin">Quản trị viên</SelectItem>
                <SelectItem value="Moderator">Điều hành</SelectItem>
                <SelectItem value="Staff">Nhân viên</SelectItem>
                <SelectItem value="User">Người dùng</SelectItem>
              </SelectContent>
            </Select>
            <Select value={statusFilter} onValueChange={(value) => { setStatusFilter(value === 'all' ? '' : value); setPagination({ ...pagination, pageNumber: 1 }); }}>
              <SelectTrigger className="w-[180px]">
                <SelectValue placeholder="Lọc theo trạng thái" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">Tất cả trạng thái</SelectItem>
                <SelectItem value="active">Hoạt động</SelectItem>
                <SelectItem value="locked">Bị khóa</SelectItem>
              </SelectContent>
            </Select>
            <Button variant="secondary" onClick={handleSearch}>
              Tìm kiếm
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* User List */}
      <Card>
        <CardHeader>
          <CardTitle className="text-lg flex items-center gap-2">
            <Users className="w-4 h-4" />
            Danh sách người dùng ({pagination.totalCount})
          </CardTitle>
        </CardHeader>
        <CardContent>
          <UserList
            users={users}
            loading={loading}
            pagination={pagination}
            onEdit={handleEdit}
            onDelete={handleDelete}
            onToggleStatus={handleToggleStatus}
            onResetPassword={handleResetPassword}
            onViewHistory={handleViewHistory}
            onPageChange={handlePageChange}
          />
        </CardContent>
      </Card>

      {/* User Form Modal */}
      <UserForm
        open={userFormOpen}
        onOpenChange={setUserFormOpen}
        onSubmit={handleFormSubmit}
        user={selectedUser}
        loading={formLoading}
      />

      {/* Change Password Modal */}
      <ChangePasswordModal
        open={passwordModalOpen}
        onOpenChange={setPasswordModalOpen}
        onSubmit={handlePasswordReset}
        user={selectedUser}
        loading={formLoading}
      />

      {/* User History Modal */}
      <UserHistory
        open={historyModalOpen}
        onOpenChange={setHistoryModalOpen}
        user={selectedUser}
      />

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Xác nhận xóa</AlertDialogTitle>
            <AlertDialogDescription>
              Bạn có chắc chắn muốn xóa tài khoản của{' '}
              <strong>{selectedUser?.fullName}</strong> không? Hành động này không thể
              hoàn tác.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Hủy</AlertDialogCancel>
            <AlertDialogAction
              onClick={confirmDelete}
              className="bg-red-600 hover:bg-red-700"
            >
              Xóa
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Status Toggle Confirmation Dialog */}
      <AlertDialog open={statusDialogOpen} onOpenChange={setStatusDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>
              {selectedUser?.isLocked ? 'Kích hoạt tài khoản' : 'Vô hiệu hóa tài khoản'}
            </AlertDialogTitle>
            <AlertDialogDescription>
              Bạn có chắc chắn muốn{' '}
              <strong>
                {selectedUser?.isLocked ? 'kích hoạt' : 'vô hiệu hóa'}
              </strong>{' '}
              tài khoản của <strong>{selectedUser?.fullName}</strong> không?
              {selectedUser?.isLocked
                ? ' Người dùng sẽ có thể đăng nhập lại.'
                : ' Người dùng sẽ không thể đăng nhập.'}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Hủy</AlertDialogCancel>
            <AlertDialogAction onClick={confirmToggleStatus}>
              Xác nhận
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
