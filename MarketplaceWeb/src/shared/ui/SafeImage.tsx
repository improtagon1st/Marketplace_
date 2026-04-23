import { type ImgHTMLAttributes, useEffect, useState } from 'react'
import { cn } from '../lib/cn'

interface SafeImageProps extends Omit<ImgHTMLAttributes<HTMLImageElement>, 'src'> {
  src?: string | null
  fallbackClassName?: string
  fallbackText?: string
}

export function SafeImage({
  src,
  alt,
  className,
  fallbackClassName,
  fallbackText = 'Нет изображения',
  ...props
}: SafeImageProps) {
  const normalizedSrc = src?.trim()
  const [hasError, setHasError] = useState(false)

  useEffect(() => {
    setHasError(false)
  }, [normalizedSrc])

  if (!normalizedSrc || hasError) {
    return (
      <div className={cn('flex items-center justify-center text-center text-sm text-slate-500', fallbackClassName)}>
        {fallbackText}
      </div>
    )
  }

  return (
    <img
      src={normalizedSrc}
      alt={alt}
      className={className}
      onError={() => setHasError(true)}
      {...props}
    />
  )
}
