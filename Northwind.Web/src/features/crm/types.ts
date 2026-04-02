export interface CustomerContact {
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

export interface CustomerCommunication {
  phone: string | null;
  fax: string | null;
  homepageUrl: string | null;
}

export interface CustomerSummary {
  customerId: string;
  companyName: string;
  contact: CustomerContact;
}

export interface CustomerDetails {
  customerId: string;
  companyName: string;
  contact: CustomerContact;
  address: Address;
  communication: CustomerCommunication;
}

export interface CustomerRequest {
  companyName: string;
  contact?: CustomerContact | null;
  address?: Address | null;
  communication?: CustomerCommunication | null;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
