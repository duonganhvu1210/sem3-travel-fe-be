import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import * as yup from 'yup';
import { profileService } from '@/services/profileService';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Loader2, Lock, Eye, EyeOff, CheckCircle, XCircle } from 'lucide-react';

const schema = yup.object().shape({
  currentPassword: yup.string().required('Mật khẩu hiện tại là bắt buộc'),
  newPassword: yup.string()
    .required('Mật khẩu mới là bắt buộc')
    .min(6, 'Mật khẩu phải có ít nhất 6 ký tự'),
  confirmNewPassword: yup.string()
    .required('Xác nhận mật khẩu là bắt buộc')
    .oneOf([yup.ref('newPassword')], 'Mật khẩu xác nhận không khớp'),
});

const SecurityTab = ({ onPasswordChange }) => {
  const [showPasswords, setShowPasswords] = useState({ current: false, new: false, confirm: false });
  const [isSaving, setIsSaving] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' });

  const { register, handleSubmit, formState: { errors }, reset } = useForm({
    resolver: yupResolver(schema),
  });

  const onSubmit = async (data) => {
    setIsSaving(true);
    setMessage({ type: '', text: '' });

    const response = await profileService.changePassword({
      currentPassword: data.currentPassword,
      newPassword: data.newPassword,
      confirmNewPassword: data.confirmNewPassword,
    });

    if (response.success) {
      setMessage({ type: 'success', text: 'Đổi mật khẩu thành công!' });
      reset();
      if (onPasswordChange) onPasswordChange();
    } else {
      setMessage({ type: 'error', text: response.message || 'Đổi mật khẩu thất bại' });
    }

    setIsSaving(false);
  };

  const PasswordInput = ({ name, label, register, error, show }) => (
    <div className="space-y-2">
      <Label htmlFor={name}>{label}</Label>
      <div className="relative">
        <Input
          id={name}
          type={show ? 'text' : 'password'}
          {...register(name)}
          className={error ? 'border-red-500 pr-10' : 'pr-10'}
        />
        <button
          type="button"
          onClick={() => setShowPasswords(prev => ({ ...prev, [name]: !prev[name] }))}
          className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
        >
          {show ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
        </button>
      </div>
      {error && <p className="text-sm text-red-500">{error.message}</p>}
    </div>
  );

  return (
    <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
      <div className="flex items-center gap-3 mb-6">
        <div className="w-10 h-10 rounded-lg bg-teal-100 flex items-center justify-center">
          <Lock className="w-5 h-5 text-teal-600" />
        </div>
        <div>
          <h2 className="text-lg font-semibold text-gray-900">Bảo mật</h2>
          <p className="text-sm text-gray-500">Quản lý mật khẩu của bạn</p>
        </div>
      </div>

      {message.text && (
        <div className={`mb-6 p-4 rounded-lg flex items-center gap-2 ${
          message.type === 'success' ? 'bg-green-50 text-green-700 border border-green-200' : 'bg-red-50 text-red-700 border border-red-200'
        }`}>
          {message.type === 'success' ? <CheckCircle className="w-5 h-5" /> : <XCircle className="w-5 h-5" />}
          {message.text}
        </div>
      )}

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 max-w-md">
        <PasswordInput name="currentPassword" label="Mật khẩu hiện tại" register={register} error={errors.currentPassword} show={showPasswords.current} />
        <PasswordInput name="newPassword" label="Mật khẩu mới" register={register} error={errors.newPassword} show={showPasswords.new} />
        <PasswordInput name="confirmNewPassword" label="Xác nhận mật khẩu mới" register={register} error={errors.confirmNewPassword} show={showPasswords.confirm} />

        <div className="pt-4">
          <Button type="submit" disabled={isSaving} className="bg-teal-600 hover:bg-teal-700">
            {isSaving ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : null}
            Đổi mật khẩu
          </Button>
        </div>
      </form>
    </div>
  );
};

export default SecurityTab;
