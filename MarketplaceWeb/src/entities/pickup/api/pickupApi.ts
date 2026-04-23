import { api } from '../../../shared/api/http'
import type { PickupPoint } from '../../../shared/types/api'

export async function getPickupPoints() {
  const { data } = await api.get<PickupPoint[]>('/pickuppoints')
  return data
}
