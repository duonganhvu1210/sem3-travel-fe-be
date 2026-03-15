import React, { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { MessageCircle, Send, Clock, CheckCircle, Loader2, ArrowLeft, ChevronRight } from 'lucide-react';
import api from '@/services/api';

const MyMessagesPage = () => {
  const [messages, setMessages] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedMessage, setSelectedMessage] = useState(null);
  const [replyContent, setReplyContent] = useState('');
  const [sendingReply, setSendingReply] = useState(false);
  const [userEmail, setUserEmail] = useState(localStorage.getItem('userEmail') || '');

  useEffect(() => {
    loadMessages();
  }, []);

  const loadMessages = async () => {
    try {
      setLoading(true);
      
      // Try to get user's email from auth context or localStorage
      const email = localStorage.getItem('userEmail') || '';
      
      if (!email) {
        // Try to get from API
        try {
          const userResponse = await api.get('/auth/me');
          if (userResponse.data.success && userResponse.data.data) {
            const emailFromApi = userResponse.data.data.email;
            setUserEmail(emailFromApi);
            localStorage.setItem('userEmail', emailFromApi);
            await fetchMessagesByEmail(emailFromApi);
            return;
          }
        } catch (e) {
          console.log('Could not get user email');
        }
        
        setError('Vui lòng đăng nhập để xem tin nhắn');
        setLoading(false);
        return;
      }

      await fetchMessagesByEmail(email);
    } catch (err) {
      console.error('Error loading messages:', err);
      setError('Có lỗi xảy ra khi tải tin nhắn');
    } finally {
      setLoading(false);
    }
  };

  const fetchMessagesByEmail = async (email) => {
    const response = await api.get(`/contacts/by-email?email=${encodeURIComponent(email)}`);
    if (response.data.success) {
      setMessages(response.data.data || []);
    } else {
      setError('Không thể tải tin nhắn');
    }
  };

  const handleSendReply = async () => {
    if (!replyContent.trim() || !selectedMessage) return;

    setSendingReply(true);
    try {
      const response = await api.post(`/contacts/${selectedMessage.contactId}/reply`, {
        replyContent: replyContent
      });

      if (response.data.success) {
        // Update the message in the list
        setMessages(prev => prev.map(m => 
          m.contactId === selectedMessage.contactId 
            ? { ...m, replyContent: replyContent, status: 'Replied' }
            : m
        ));
        
        // Update selected message
        setSelectedMessage(prev => ({
          ...prev,
          replyContent: replyContent,
          status: 'Replied'
        }));
        
        setReplyContent('');
        alert('Tin nhắn đã được gửi!');
      }
    } catch (err) {
      console.error('Error sending reply:', err);
      alert('Có lỗi xảy ra khi gửi tin nhắn');
    } finally {
      setSendingReply(false);
    }
  };

  const getStatusBadge = (status) => {
    const statusConfig = {
      'Unread': { bg: 'bg-red-100', text: 'text-red-700', label: 'Chưa đọc' },
      'Read': { bg: 'bg-blue-100', text: 'text-blue-700', label: 'Đã đọc' },
      'Replied': { bg: 'bg-green-100', text: 'text-green-700', label: 'Đã phản hồi' },
      'Closed': { bg: 'bg-gray-100', text: 'text-gray-700', label: 'Đã đóng' }
    };
    
    const config = statusConfig[status] || statusConfig['Unread'];
    return (
      <span className={`px-2 py-1 rounded-full text-xs font-medium ${config.bg} ${config.text}`}>
        {config.label}
      </span>
    );
  };

  const getRequestTypeLabel = (type) => {
    const labels = {
      'General': 'Chung',
      'Booking': 'Đặt tour',
      'Consulting': 'Tư vấn',
      'Feedback': 'Phản hồi',
      'Callback': 'Yêu cầu gọi lại'
    };
    return labels[type] || type;
  };

  const formatDate = (dateString) => {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('vi-VN', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <Loader2 className="w-8 h-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="container mx-auto px-4">
        {/* Header */}
        <div className="mb-8">
          <Link to="/" className="flex items-center gap-2 text-gray-600 hover:text-primary mb-4">
            <ArrowLeft className="w-5 h-5" />
            <span>Quay lại</span>
          </Link>
          <h1 className="text-3xl font-bold text-gray-800 mb-2">Tin nhắn của tôi</h1>
          <p className="text-gray-600">Quản lý và theo dõi các yêu cầu liên hệ</p>
        </div>

        {error && (
          <div className="bg-red-50 text-red-600 p-4 rounded-lg mb-4">
            {error}
            <Link to="/login" className="underline ml-2">Đăng nhập</Link>
          </div>
        )}

        {!error && messages.length === 0 && !loading && (
          <div className="text-center py-16 bg-white rounded-xl shadow-sm">
            <MessageCircle className="w-16 h-16 mx-auto text-gray-300 mb-4" />
            <h3 className="text-xl font-semibold text-gray-600 mb-2">Chưa có tin nhắn</h3>
            <p className="text-gray-500 mb-6">Hãy gửi yêu cầu liên hệ cho chúng tôi!</p>
            <Link
              to="/contact"
              className="inline-block px-6 py-3 bg-primary text-white rounded-lg hover:bg-teal-700"
            >
              Liên hệ ngay
            </Link>
          </div>
        )}

        {messages.length > 0 && (
          <div className="grid lg:grid-cols-3 gap-6">
            {/* Messages List */}
            <div className="lg:col-span-1 bg-white rounded-xl shadow-sm overflow-hidden">
              <div className="p-4 border-b bg-gray-50">
                <h2 className="font-semibold text-gray-800">Danh sách tin nhắn</h2>
                <p className="text-sm text-gray-500">{messages.length} tin nhắn</p>
              </div>
              <div className="divide-y max-h-[600px] overflow-y-auto">
                {messages.map((message) => (
                  <button
                    key={message.contactId}
                    onClick={() => setSelectedMessage(message)}
                    className={`w-full p-4 text-left hover:bg-gray-50 transition-colors ${
                      selectedMessage?.contactId === message.contactId ? 'bg-teal-50 border-l-4 border-primary' : ''
                    }`}
                  >
                    <div className="flex items-start justify-between mb-1">
                      <span className="font-medium text-gray-800 line-clamp-1">
                        {getRequestTypeLabel(message.requestType)}
                      </span>
                      {getStatusBadge(message.status)}
                    </div>
                    <p className="text-sm text-gray-600 line-clamp-2 mb-2">
                      {message.messageContent}
                    </p>
                    <p className="text-xs text-gray-400">
                      {formatDate(message.createdAt)}
                    </p>
                  </button>
                ))}
              </div>
            </div>

            {/* Message Detail */}
            <div className="lg:col-span-2 bg-white rounded-xl shadow-sm overflow-hidden">
              {selectedMessage ? (
                <div className="h-full flex flex-col">
                  <div className="p-6 border-b">
                    <div className="flex items-start justify-between mb-4">
                      <div>
                        <h3 className="text-xl font-bold text-gray-800 mb-1">
                          {getRequestTypeLabel(selectedMessage.requestType)}
                        </h3>
                        <p className="text-sm text-gray-500">
                          Gửi lúc {formatDate(selectedMessage.createdAt)}
                        </p>
                      </div>
                      {getStatusBadge(selectedMessage.status)}
                    </div>
                    
                    <div className="grid grid-cols-2 gap-4 text-sm">
                      {selectedMessage.serviceType && (
                        <div>
                          <span className="text-gray-500">Dịch vụ: </span>
                          <span className="font-medium">{selectedMessage.serviceType}</span>
                        </div>
                      )}
                      {selectedMessage.expectedDate && (
                        <div>
                          <span className="text-gray-500">Ngày dự kiến: </span>
                          <span className="font-medium">{formatDate(selectedMessage.expectedDate)}</span>
                        </div>
                      )}
                      {selectedMessage.participantCount && (
                        <div>
                          <span className="text-gray-500">Số người: </span>
                          <span className="font-medium">{selectedMessage.participantCount}</span>
                        </div>
                      )}
                    </div>
                  </div>

                  {/* Original Message */}
                  <div className="p-6 bg-gray-50">
                    <h4 className="font-medium text-gray-700 mb-2">Tin nhắn của bạn:</h4>
                    <div className="bg-white p-4 rounded-lg border">
                      <p className="text-gray-800 whitespace-pre-wrap">{selectedMessage.messageContent}</p>
                    </div>
                  </div>

                  {/* Reply from Admin */}
                  {selectedMessage.replyContent && (
                    <div className="p-6 bg-green-50 border-t">
                      <h4 className="font-medium text-green-800 mb-2 flex items-center gap-2">
                        <CheckCircle className="w-4 h-4" />
                        Phản hồi từ KarnelTravels:
                      </h4>
                      <div className="bg-white p-4 rounded-lg border border-green-200">
                        <p className="text-gray-800 whitespace-pre-wrap">{selectedMessage.replyContent}</p>
                        {selectedMessage.repliedAt && (
                          <p className="text-xs text-gray-400 mt-2">
                            Phản hồi lúc {formatDate(selectedMessage.repliedAt)}
                          </p>
                        )}
                      </div>
                    </div>
                  )}

                  {/* Reply Input */}
                  <div className="p-6 border-t mt-auto">
                    <h4 className="font-medium text-gray-700 mb-2">Tiếp tục trò chuyện:</h4>
                    <textarea
                      value={replyContent}
                      onChange={(e) => setReplyContent(e.target.value)}
                      placeholder="Nhập tin nhắn..."
                      rows={3}
                      className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary focus:border-transparent resize-none"
                    />
                    <button
                      onClick={handleSendReply}
                      disabled={sendingReply || !replyContent.trim()}
                      className="mt-3 px-6 py-2 bg-primary text-white rounded-lg hover:bg-teal-700 disabled:bg-gray-400 disabled:cursor-not-allowed flex items-center gap-2"
                    >
                      {sendingReply ? (
                        <>
                          <Loader2 className="w-4 h-4 animate-spin" />
                          Đang gửi...
                        </>
                      ) : (
                        <>
                          <Send className="w-4 h-4" />
                          Gửi tin nhắn
                        </>
                      )}
                    </button>
                  </div>
                </div>
              ) : (
                <div className="h-full flex items-center justify-center text-gray-400">
                  <div className="text-center">
                    <MessageCircle className="w-16 h-16 mx-auto mb-4" />
                    <p>Chọn một tin nhắn để xem chi tiết</p>
                  </div>
                </div>
              )}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default MyMessagesPage;
