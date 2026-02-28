export interface UserDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: UserDto;
}

export interface UserDetailDto {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roleId: number;
  roleName: string;
  isActive: boolean;
  createdAt: string;
}

export interface LobDto {
  id: string;
  name: string;
  code: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
}

export interface ProductDto {
  id: string;
  lobId: string;
  lobName: string;
  insurerId: string;
  insurerName: string;
  name: string;
  code: string;
  description?: string;
  version: string;
  status: string;
  effectiveDate: string;
  expiryDate?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CoverageDto {
  id: string;
  productId: string;
  name: string;
  code: string;
  description?: string;
  isMandatory: boolean;
  sequenceNo: number;
  createdAt: string;
  covers: CoverDto[];
}

export interface CoverDto {
  id: string;
  coverageId: string;
  name: string;
  code: string;
  description?: string;
  isMandatory: boolean;
  sequenceNo: number;
  createdAt: string;
}

export interface LimitDto {
  id: string;
  coverId: string;
  limitType: string;
  minAmount: number;
  maxAmount: number;
  defaultAmount: number;
  currency: string;
  isActive: boolean;
}

export interface DeductibleDto {
  id: string;
  coverId: string;
  deductibleType: string;
  minAmount: number;
  maxAmount: number;
  defaultAmount: number;
  currency: string;
  isActive: boolean;
}

export interface PremiumDto {
  id: string;
  coverId: string;
  premiumType: string;
  baseRate?: number;
  flatAmount?: number;
  minPremium?: number;
  calculationBasis?: string;
  currency: string;
  isActive: boolean;
}

export interface ModifierDto {
  id: string;
  coverId?: string;
  productId?: string;
  name: string;
  code: string;
  modifierType: string;
  valueType: string;
  minValue: number;
  maxValue: number;
  defaultValue?: number;
  isMandatory: boolean;
  description?: string;
  isActive: boolean;
}

export interface InsurerDto {
  id: string;
  name: string;
  code: string;
  licenseNo?: string;
  address?: string;
  phone?: string;
  email?: string;
  isActive: boolean;
  createdAt: string;
}

export interface UnderwriterDto {
  id: string;
  userId: string;
  userName: string;
  userEmail: string;
  licenseNo?: string;
  specialization?: string;
  authorityLimit?: number;
  createdAt: string;
}

export interface BrokerDto {
  id: string;
  userId: string;
  userName: string;
  userEmail: string;
  insurerId?: string;
  insurerName?: string;
  companyName: string;
  licenseNo?: string;
  commissionRate?: number;
  isActive: boolean;
  createdAt: string;
}

export interface QuoteDto {
  id: string;
  productId: string;
  productName: string;
  brokerId?: string;
  brokerName?: string;
  underwriterId?: string;
  underwriterName?: string;
  insuredName: string;
  insuredEmail?: string;
  insuredPhone?: string;
  status: string;
  currency: string;
  basePremium?: number;
  totalPremium?: number;
  validUntil?: string;
  notes?: string;
  createdAt: string;
  covers: QuoteCoverDto[];
  modifiers: QuoteModifierDto[];
}

export interface QuoteCoverDto {
  id: string;
  coverId: string;
  coverName: string;
  isSelected: boolean;
  selectedLimit?: number;
  selectedDeductible?: number;
  calculatedPremium?: number;
  basisValue?: number;
}

export interface QuoteModifierDto {
  id: string;
  modifierId: string;
  modifierName: string;
  appliedValue: number;
  premiumImpact?: number;
}
