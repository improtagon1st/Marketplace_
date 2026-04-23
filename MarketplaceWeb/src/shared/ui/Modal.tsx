import { Dialog, DialogPanel, DialogTitle } from '@headlessui/react'
import { type PropsWithChildren } from 'react'
import { Button } from './Button'

interface ModalProps extends PropsWithChildren {
  isOpen: boolean
  title: string
  onClose: () => void
}

export function Modal({ isOpen, title, onClose, children }: ModalProps) {
  return (
    <Dialog open={isOpen} onClose={onClose} className="relative z-50">
      <div className="fixed inset-0 bg-black/30" aria-hidden="true" />
      <div className="fixed inset-0 flex items-center justify-center p-4">
        <DialogPanel className="w-full max-w-lg rounded-2xl bg-white p-6 shadow-card">
          <DialogTitle className="text-lg font-bold text-ink">{title}</DialogTitle>
          <div className="mt-4">{children}</div>
          <div className="mt-6 flex justify-end">
            <Button variant="ghost" onClick={onClose}>
              Закрыть
            </Button>
          </div>
        </DialogPanel>
      </div>
    </Dialog>
  )
}
