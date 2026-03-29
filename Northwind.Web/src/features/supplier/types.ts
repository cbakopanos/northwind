export interface SupplierContact {
  contactName: string | null;
  contactTitle: string | null;
}

export interface Address {
  addressLine: string | null;
  city: string | null;
  region: string | null;
  postalCode: string | null;
  country: string | null;
}

export interface SupplierCommunication {
  phone: string | null;
  fax: string | null;
  homepageUrl: string | null;
}

export interface SupplierSummary {
  supplierId: number;
  companyName: string;
  contact: SupplierContact;
}

export interface SupplierDetails {
  supplierId: number;
  companyName: string;
  contact: SupplierContact;
  address: Address;
  communication: SupplierCommunication;
}

export interface SupplierRequest {
  companyName: string;
  contact?: SupplierContact | null;
  address?: Address | null;
  communication?: SupplierCommunication | null;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
