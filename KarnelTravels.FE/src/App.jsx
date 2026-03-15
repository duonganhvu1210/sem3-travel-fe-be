import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext/AuthContext';
import { CompareProvider } from './contexts/CompareContext';
import { BookingProvider } from './context/BookingContext';
import AppRoutes from './routes/AppRoutes';
import { Toaster } from 'react-hot-toast';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <CompareProvider>
          <BookingProvider>
            <AppRoutes />
            <Toaster 
              position="top-right"
              toastOptions={{
                duration: 3000,
                style: {
                  background: '#363636',
                  color: '#fff',
                },
                success: {
                  iconTheme: {
                    primary: '#10b981',
                    secondary: '#fff',
                  },
                },
                error: {
                  iconTheme: {
                    primary: '#ef4444',
                    secondary: '#fff',
                  },
                },
              }}
            />
          </BookingProvider>
        </CompareProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;
