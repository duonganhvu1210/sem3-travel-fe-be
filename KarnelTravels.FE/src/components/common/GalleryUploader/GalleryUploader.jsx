import { useState, useRef } from 'react';
import { Upload, X, Image as ImageIcon, Loader2 } from 'lucide-react';

const GalleryUploader = ({ 
  images = [], 
  onChange, 
  maxImages = 10,
  disabled = false,
  placeholder = "Nhập URL hình ảnh hoặc kéo thả"
}) => {
  const [isLoading, setIsLoading] = useState(false);
  const [dragOver, setDragOver] = useState(false);
  const fileInputRef = useRef(null);

  const handleAddImage = async (url) => {
    if (!url.trim()) return;
    if (images.length >= maxImages) return;
    
    // Validate URL
    try {
      new URL(url);
    } catch {
      return;
    }

    if (!images.includes(url)) {
      onChange([...images, url]);
    }
  };

  const handleRemoveImage = (index) => {
    const newImages = [...images];
    newImages.splice(index, 1);
    onChange(newImages);
  };

  const handleDragOver = (e) => {
    e.preventDefault();
    if (!disabled) setDragOver(true);
  };

  const handleDragLeave = () => {
    setDragOver(false);
  };

  const handleDrop = async (e) => {
    e.preventDefault();
    setDragOver(false);
    if (disabled) return;

    const files = Array.from(e.dataTransfer.files);
    const imageFiles = files.filter(file => file.type.startsWith('image/'));
    
    for (const file of imageFiles) {
      // In a real app, you'd upload to a server/cloud storage
      // For now, we'll use object URL
      const objectUrl = URL.createObjectURL(file);
      if (images.length < maxImages) {
        handleAddImage(objectUrl);
      }
    }
  };

  const handleFileSelect = async (e) => {
    const files = Array.from(e.target.files);
    const imageFiles = files.filter(file => file.type.startsWith('image/'));
    
    for (const file of imageFiles) {
      const objectUrl = URL.createObjectURL(file);
      if (images.length < maxImages) {
        handleAddImage(objectUrl);
      }
    }
  };

  const handleUrlInput = () => {
    const url = prompt('Nhập URL hình ảnh:');
    if (url) {
      handleAddImage(url);
    }
  };

  return (
    <div className="space-y-3">
      {/* Image Grid */}
      {images.length > 0 && (
        <div className="grid grid-cols-4 gap-3">
          {images.map((url, index) => (
            <div key={index} className="relative group aspect-square rounded-lg overflow-hidden border border-gray-200">
              <img 
                src={url} 
                alt={`Image ${index + 1}`} 
                className="w-full h-full object-cover"
                onError={(e) => {
                  e.target.src = 'https://via.placeholder.com/150?text=Error';
                }}
              />
              {!disabled && (
                <button
                  type="button"
                  onClick={() => handleRemoveImage(index)}
                  className="absolute top-1 right-1 p-1 bg-red-500 text-white rounded-full opacity-0 group-hover:opacity-100 transition-opacity"
                >
                  <X className="w-3 h-3" />
                </button>
              )}
              {index === 0 && (
                <span className="absolute bottom-1 left-1 px-1.5 py-0.5 bg-teal-500 text-white text-xs rounded">Bìa</span>
              )}
            </div>
          ))}
        </div>
      )}

      {/* Upload Area */}
      {!disabled && images.length < maxImages && (
        <div 
          className={`
            border-2 border-dashed rounded-lg p-6 text-center cursor-pointer transition-colors
            ${dragOver ? 'border-teal-500 bg-teal-50' : 'border-gray-300 hover:border-gray-400'}
            ${disabled ? 'opacity-50 cursor-not-allowed' : ''}
          `}
          onDragOver={handleDragOver}
          onDragLeave={handleDragLeave}
          onDrop={handleDrop}
          onClick={() => !disabled && fileInputRef.current?.click()}
        >
          <input
            ref={fileInputRef}
            type="file"
            accept="image/*"
            multiple
            className="hidden"
            onChange={handleFileSelect}
            disabled={disabled}
          />
          
          {isLoading ? (
            <Loader2 className="w-8 h-8 mx-auto text-gray-400 animate-spin" />
          ) : (
            <>
              <ImageIcon className="w-8 h-8 mx-auto text-gray-400 mb-2" />
              <p className="text-sm text-gray-500">{placeholder}</p>
              <p className="text-xs text-gray-400 mt-1">
                Hoặc <button type="button" onClick={(e) => { e.stopPropagation(); handleUrlInput(); }} className="text-teal-500 hover:underline">nhập URL</button>
              </p>
            </>
          )}
        </div>
      )}

      <p className="text-xs text-gray-400">
        {images.length}/{maxImages} hình ảnh
      </p>
    </div>
  );
};

export default GalleryUploader;
