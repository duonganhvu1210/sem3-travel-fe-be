import { createContext, useContext, useState, useCallback } from 'react';

const BookingContext = createContext(null);

export const useBooking = () => {
  const context = useContext(BookingContext);
  if (!context) {
    throw new Error('useBooking must be used within a BookingProvider');
  }
  return context;
};

export const BookingProvider = ({ children }) => {
  const [cart, setCart] = useState([]);
  const [currentBooking, setCurrentBooking] = useState(null);
  const [bookingStep, setBookingStep] = useState(1);

  // F318: Add to cart for combo booking
  const addToCart = useCallback((item) => {
    setCart(prev => {
      const existing = prev.find(i => i.serviceId === item.serviceId && i.serviceType === item.serviceType);
      if (existing) {
        return prev.map(i => 
          (i.serviceId === item.serviceId && i.serviceType === item.serviceType)
            ? { ...i, ...item }
            : i
        );
      }
      return [...prev, { ...item, id: Date.now() }];
    });
  }, []);

  // Remove from cart
  const removeFromCart = useCallback((itemId) => {
    setCart(prev => prev.filter(item => item.id !== itemId));
  }, []);

  // Clear cart
  const clearCart = useCallback(() => {
    setCart([]);
    setCurrentBooking(null);
    setBookingStep(1);
  }, []);

  // Set current booking details
  const setBookingDetails = useCallback((details) => {
    setCurrentBooking(prev => ({ ...prev, ...details }));
  }, []);

  // Next step
  const nextStep = useCallback(() => {
    setBookingStep(prev => Math.min(prev + 1, 4));
  }, []);

  // Previous step
  const prevStep = useCallback(() => {
    setBookingStep(prev => Math.max(prev - 1, 1));
  }, []);

  // Go to specific step
  const goToStep = useCallback((step) => {
    if (step >= 1 && step <= 4) {
      setBookingStep(step);
    }
  }, []);

  // Calculate total price of cart
  const calculateCartTotal = useCallback(() => {
    return cart.reduce((total, item) => total + (item.finalAmount || 0), 0);
  }, [cart]);

  const value = {
    // Cart state
    cart,
    addToCart,
    removeFromCart,
    clearCart,
    calculateCartTotal,
    
    // Current booking
    currentBooking,
    setCurrentBooking,
    setBookingDetails,
    
    // Steps
    bookingStep,
    nextStep,
    prevStep,
    goToStep,
    
    // Helpers
    isCartEmpty: cart.length === 0,
    cartItemCount: cart.length
  };

  return (
    <BookingContext.Provider value={value}>
      {children}
    </BookingContext.Provider>
  );
};

export default BookingContext;
