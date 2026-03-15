import { createContext, useContext, useState, useEffect } from 'react';

const CompareContext = createContext();

export const CompareProvider = ({ children }) => {
  const [compareItems, setCompareItems] = useState(() => {
    const saved = localStorage.getItem('compareItems');
    return saved ? JSON.parse(saved) : [];
  });

  useEffect(() => {
    localStorage.setItem('compareItems', JSON.stringify(compareItems));
  }, [compareItems]);

  const addToCompare = (item) => {
    setCompareItems(prev => {
      if (prev.length >= 3) return prev;
      const exists = prev.some(i => i.tourId === item.tourId);
      if (exists) return prev;
      return [...prev, item];
    });
  };

  const removeFromCompare = (item) => {
    setCompareItems(prev => prev.filter(i => i.tourId !== item.tourId));
  };

  const clearCompare = () => {
    setCompareItems([]);
  };

  const isInCompare = (tourId) => {
    return compareItems.some(i => i.tourId === tourId);
  };

  return (
    <CompareContext.Provider value={{
      compareItems,
      addToCompare,
      removeFromCompare,
      clearCompare,
      isInCompare
    }}>
      {children}
    </CompareContext.Provider>
  );
};

export const useCompare = () => {
  const context = useContext(CompareContext);
  if (!context) {
    throw new Error('useCompare must be used within CompareProvider');
  }
  return context;
};
