import { useState } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import {
  MoreHorizontal,
  User,
  Shield,
  Key,
  Ban,
  History,
  Edit,
  Trash2,
  CheckCircle,
  XCircle,
} from 'lucide-react';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';

const roleColors = {
  Admin: 'bg-purple-100 text-purple-800 border-purple-200',
  Moderator: 'bg-blue-100 text-blue-800 border-blue-200',
  Staff: 'bg-cyan-100 text-cyan-800 border-cyan-200',
  User: 'bg-gray-100 text-gray-800 border-gray-200',
};

const roleLabels = {
  Admin: 'Quản trị viên',
  Moderator: 'Điều hành',
  Staff: 'Nhân viên',
  User: 'Người dùng',
};

export default function UserList({
  users = [],
  loading = false,
  onEdit,
  onDelete,
  onToggleStatus,
  onResetPassword,
  onViewHistory,
  onPageChange,
  pagination,
}) {
  const getRoleBadge = (role) => {
    const colorClass = roleColors[role] || roleColors.User;
    return (
      <Badge className={`${colorClass} border`}>
        {roleLabels[role] || role}
      </Badge>
    );
  };

  const getStatusBadge = (isLocked) => {
    if (isLocked) {
      return (
        <Badge variant="destructive" className="bg-red-100 text-red-800 border-red-200">
          <XCircle className="w-3 h-3 mr-1" />
          Bị khóa
        </Badge>
      );
    }
    return (
      <Badge className="bg-green-100 text-green-800 border-green-200">
        <CheckCircle className="w-3 h-3 mr-1" />
        Hoạt động
        </Badge>
    );
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
      </div>
    );
  }

  if (users.length === 0) {
    return (
      <div className="flex flex-col items-center justify-center h-64 text-muted-foreground">
        <User className="w-12 h-12 mb-4 opacity-50" />
        <p>Không có người dùng nào</p>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead className="w-[50px]">#</TableHead>
              <TableHead>Người dùng</TableHead>
              <TableHead>Email</TableHead>
              <TableHead>Số điện thoại</TableHead>
              <TableHead>Vai trò</TableHead>
              <TableHead>Trạng thái</TableHead>
              <TableHead>Ngày tạo</TableHead>
              <TableHead className="w-[80px]">Thao tác</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {users.map((user, index) => (
              <TableRow key={user.userId}>
                <TableCell className="font-medium">
                  {(pagination?.pageNumber - 1) * pagination?.pageSize + index + 1}
                </TableCell>
                <TableCell>
                  <div className="flex items-center gap-3">
                    <div className="w-8 h-8 rounded-full bg-primary/10 flex items-center justify-center">
                      {user.avatar ? (
                        <img
                          src={user.avatar}
                          alt={user.fullName}
                          className="w-8 h-8 rounded-full object-cover"
                        />
                      ) : (
                        <User className="w-4 h-4 text-primary" />
                      )}
                    </div>
                    <span className="font-medium">{user.fullName}</span>
                  </div>
                </TableCell>
                <TableCell>{user.email}</TableCell>
                <TableCell>{user.phoneNumber || '-'}</TableCell>
                <TableCell>{getRoleBadge(user.roleName)}</TableCell>
                <TableCell>{getStatusBadge(user.isLocked)}</TableCell>
                <TableCell>
                  {new Date(user.createdAt).toLocaleDateString('vi-VN')}
                </TableCell>
                <TableCell>
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" className="h-8 w-8 p-0">
                        <MoreHorizontal className="h-4 w-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      <DropdownMenuItem onClick={() => onEdit(user)}>
                        <Edit className="mr-2 h-4 w-4" />
                        Chỉnh sửa
                      </DropdownMenuItem>
                      <DropdownMenuItem onClick={() => onViewHistory(user)}>
                        <History className="mr-2 h-4 w-4" />
                        Lịch sử đơn đặt
                      </DropdownMenuItem>
                      <DropdownMenuItem onClick={() => onResetPassword(user)}>
                        <Key className="mr-2 h-4 w-4" />
                        Đặt lại mật khẩu
                      </DropdownMenuItem>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem
                        onClick={() => onToggleStatus(user)}
                        className={user.isLocked ? 'text-green-600' : 'text-orange-600'}
                      >
                        {user.isLocked ? (
                          <>
                            <CheckCircle className="mr-2 h-4 w-4" />
                            Kích hoạt tài khoản
                          </>
                        ) : (
                          <>
                            <Ban className="mr-2 h-4 w-4" />
                            Vô hiệu hóa
                          </>
                        )}
                      </DropdownMenuItem>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem
                        onClick={() => onDelete(user)}
                        className="text-red-600 focus:text-red-600"
                      >
                        <Trash2 className="mr-2 h-4 w-4" />
                        Xóa tài khoản
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      {/* Pagination */}
      {pagination && pagination.totalPages > 1 && (
        <div className="flex items-center justify-between">
          <div className="text-sm text-muted-foreground">
            Trang {pagination.pageNumber} / {pagination.totalPages}
            <span className="ml-2">(Tổng: {pagination.totalCount} người dùng)</span>
          </div>
          <div className="flex items-center gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => onPageChange(pagination.pageNumber - 1)}
              disabled={!pagination.hasPreviousPage}
            >
              Trước
            </Button>
            <Button
              variant="outline"
              size="sm"
              onClick={() => onPageChange(pagination.pageNumber + 1)}
              disabled={!pagination.hasNextPage}
            >
              Sau
            </Button>
          </div>
        </div>
      )}
    </div>
  );
}
