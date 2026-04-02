export interface InventoryLevel {
  unitsInStock: number;
  unitsOnOrder: number;
}

export interface CategorySummary {
  categoryId: number;
  categoryName: string;
  hasPicture: boolean;
}

export interface CategoryDetails {
  categoryId: number;
  categoryName: string;
  description: string | null;
  hasPicture: boolean;
}

export interface CategoryRequest {
  categoryName: string;
  description: string | null;
}

export interface ProductSummary {
  productId: number;
  productName: string;
  categoryName: string | null;
  unitPrice: number;
  isDiscontinued: boolean;
}

export interface Supplier {
  supplierId: number;
  displayName: string;
}

export interface ProductDetails {
  productId: number;
  productName: string;
  supplier: Supplier | null;
  categoryId: number | null;
  categoryName: string | null;
  quantityPerUnit: string | null;
  unitPrice: number;
  inventory: InventoryLevel;
  reorderLevel: number;
  isDiscontinued: boolean;
}

export interface ProductRequest {
  productName: string;
  supplierId: number | null;
  categoryId: number | null;
  quantityPerUnit: string | null;
  unitPrice: number;
  inventory: InventoryLevel | null;
  reorderLevel: number;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
