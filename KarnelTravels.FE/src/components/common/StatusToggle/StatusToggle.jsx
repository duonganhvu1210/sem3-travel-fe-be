import { useState } from 'react';
import { Loader2, CheckCircle, XCircle } from 'lucide-react';

const StatusToggle = ({ 
  isActive, 
  onChange, 
  disabled = false,
  activeLabel = "Hoạt động",
  inactiveLabel = "Tạm dừng",
  size = "default" // "small" | "default" | "large"
}) => {
  const [isLoading, setIsLoading] = useState(false);

  const handleToggle = async () => {
    if (disabled || isLoading) return;
    
    setIsLoading(true);
    try {
      await onChange(!isActive);
    } finally {
      setIsLoading(false);
    }
  };

  const sizeClasses = {
    small: {
      container: "w-10 h-5",
      circle: "w-4 h-4",
      active: "translate-x-5",
      inactive: "translate-x-0.5"
    },
    default: {
      container: "w-12 h-6",
      circle: "w-5 h-5",
      active: "translate-x-6",
      inactive: "translate-x-0.5"
    },
    large: {
      container: "w-14 h-7",
      circle: "w-6 h-6",
      active: "translate-x-7",
      inactive: "translate-x-0.5"
    }
  };

  const sizes = sizeClasses[size] || sizeClasses.default;

  return (
    <div className="flex items-center gap-3">
      <button
        type="button"
        onClick={handleToggle}
        disabled={disabled || isLoading}
        className={`
          relative inline-flex shrink-0 ${sizes.container} rounded-full transition-colors duration-200 ease-in-out
          ${isActive ? 'bg-teal-500' : 'bg-gray-300'}
          ${disabled || isLoading ? 'opacity-50 cursor-not-allowed' : 'cursor-pointer'}
        `}
      >
        <span className="sr-only">Toggle status</span>
        <span
          className={`
            ${sizes.circle} pointer-events-none inline-block rounded-full bg-white shadow transform ring-0 transition duration-200 ease-in-out
            ${isActive ? sizes.active : sizes.inactive}
          `}
        />
      </button>
      
      {isLoading ? (
        <Loader2 className="w-4 h-4 animate-spin text-gray-400" />
      ) : isActive ? (
        <span className="flex items-center gap-1 text-sm text-teal-600">
          <CheckCircle className="w-4 h-4" />
          {activeLabel}
        </span>
      ) : (
        <span className="flex items-center gap-1 text-sm text-gray-500">
          <XCircle className="w-4 h-4" />
          {inactiveLabel}
        </span>
      )}
    </div>
  );
};

export default StatusToggle;
