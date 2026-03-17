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
      toast.error('Cannot load user list');
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
        toast.error(response.message || 'Cannot create user');
      }
    } catch (error) {
      console.error('Error creating user:', error);
      toast.error(error.response?.data?.message || 'Cannot create user');
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
        toast.error(response.message || 'Cannot update user');
      }
    } catch (error) {
      console.error('Error updating user:', error);
        toast.error(error.response?.data?.message || 'Cannot update user');
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
        toast.success('User deleted successfully');
        fetchUsers();
      } else {
        toast.error(response.message || 'Cannot delete user');
      }
    } catch (error) {
      console.error('Error deleting user:', error);
      toast.error('Cannot delete user');
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
        toast.error(response.message || 'Cannot change status');
      }
    } catch (error) {
      console.error('Error toggling user status:', error);
      toast.error('Cannot change status');
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
        toast.error(response.message || 'Cannot reset password');
      }
    } catch (error) {
      console.error('Error resetting password:', error);
      toast.error('Cannot reset password');
    }
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">User Management</h1>
          <p className="text-muted-foreground">
            Manage user accounts and permissions
          </p>
        </div>
        <Button onClick={() => { setSelectedUser(null); setUserFormOpen(true); }}>
          <Plus className="w-4 h-4 mr-2" />
          Thêm user
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
                  placeholder="Search by name, email, phone..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                  className="pl-9"
                />
              </div>
            </div>
            <Select value={roleFilter} onValueChange={(value) => { setRoleFilter(value === 'all' ? '' : value); setPagination({ ...pagination, pageNumber: 1 }); }}>
              <SelectTrigger className="w-[180px]">
                <SelectValue placeholder="Filter by role" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Roles</SelectItem>
                <SelectItem value="Admin">Administrator</SelectItem>
                <SelectItem value="Moderator">Moderator</SelectItem>
                <SelectItem value="Staff">Staff</SelectItem>
                <SelectItem value="User">User</SelectItem>
              </SelectContent>
            </Select>
            <Select value={statusFilter} onValueChange={(value) => { setStatusFilter(value === 'all' ? '' : value); setPagination({ ...pagination, pageNumber: 1 }); }}>
              <SelectTrigger className="w-[180px]">
                <SelectValue placeholder="Filter by status" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Status</SelectItem>
                <SelectItem value="active">Active</SelectItem>
                <SelectItem value="locked">Locked</SelectItem>
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
            <AlertDialogTitle>Confirm delete</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete the account of{' '}
              <strong>{selectedUser?.fullName}</strong>? This action cannot be
              hoàn tác.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction
              onClick={confirmDelete}
              className="bg-red-600 hover:bg-red-700"
            >
              Delete
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>

      {/* Status Toggle Confirmation Dialog */}
      <AlertDialog open={statusDialogOpen} onOpenChange={setStatusDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>
              {selectedUser?.isLocked ? 'Activate account' : 'Deactivate account'}
            </AlertDialogTitle>
            <AlertDialogDescription>
              Bạn có chắc chắn muốn{' '}
              <strong>
                {selectedUser?.isLocked ? 'activate' : 'deactivate'}
              </strong>{' '}
              account of <strong>{selectedUser?.fullName}</strong>?
              {selectedUser?.isLocked
                ? ' User will be able to login again.'
                : ' User will not be able to login.'}
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction onClick={confirmToggleStatus}>
              Confirm
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </div>
  );
}
