import toast, { Toaster } from 'react-hot-toast'

export function Toast() {
  return (
    <Toaster
      position="top-right"
      toastOptions={{
        duration: 3500,
        style: {
          background: '#111827',
          color: '#f9fafb',
          border: '1px solid #374151',
        },
      }}
    />
  )
}

export { toast }
