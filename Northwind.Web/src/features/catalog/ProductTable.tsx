import { Pencil, Ban, RotateCcw } from "lucide-react";
import type { ProductSummary } from "./types";

interface ProductTableProps {
  products: ProductSummary[];
  onEdit: (productId: number) => void;
  onDiscontinue: (productId: number) => void;
  onReinstate: (productId: number) => void;
}

export function ProductTable({ products, onEdit, onDiscontinue, onReinstate }: ProductTableProps) {
  return (
    <div className="overflow-x-auto rounded-lg border">
      <table className="w-full text-left text-sm">
        <thead className="border-b bg-gray-50 text-xs uppercase text-gray-500">
          <tr>
            <th className="px-6 py-3 w-16 text-center">#</th>
            <th className="px-6 py-3">Product Name</th>
            <th className="px-6 py-3">Category</th>
            <th className="px-6 py-3 text-right">Unit Price</th>
            <th className="px-6 py-3 text-center">Status</th>
            <th className="px-6 py-3 text-right">Actions</th>
          </tr>
        </thead>
        <tbody className="divide-y">
          {products.map((p) => (
            <tr key={p.productId} className="hover:bg-gray-50">
              <td className="px-6 py-4 text-center">
                <span className="inline-flex h-6 min-w-6 items-center justify-center rounded-full bg-gray-100 px-2 text-xs font-medium text-gray-500">
                  {p.productId}
                </span>
              </td>
              <td className="px-6 py-4 font-medium text-gray-900">
                {p.productName}
              </td>
              <td className="px-6 py-4 text-gray-600">
                {p.categoryName ?? "—"}
              </td>
              <td className="px-6 py-4 text-right font-mono text-gray-700">
                ${p.unitPrice.toFixed(2)}
              </td>
              <td className="px-6 py-4 text-center">
                {p.isDiscontinued ? (
                  <span className="inline-flex items-center rounded-full bg-red-50 px-2.5 py-0.5 text-xs font-medium text-red-700">
                    Discontinued
                  </span>
                ) : (
                  <span className="inline-flex items-center rounded-full bg-green-50 px-2.5 py-0.5 text-xs font-medium text-green-700">
                    Active
                  </span>
                )}
              </td>
              <td className="px-6 py-4 text-right">
                <div className="inline-flex gap-1">
                  <button
                    onClick={() => onEdit(p.productId)}
                    className="rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-700"
                    title="Edit product"
                  >
                    <Pencil className="h-4 w-4" />
                  </button>
                  {p.isDiscontinued ? (
                    <button
                      onClick={() => onReinstate(p.productId)}
                      className="rounded p-1 text-gray-400 hover:bg-green-50 hover:text-green-700"
                      title="Reinstate product"
                    >
                      <RotateCcw className="h-4 w-4" />
                    </button>
                  ) : (
                    <button
                      onClick={() => onDiscontinue(p.productId)}
                      className="rounded p-1 text-gray-400 hover:bg-red-50 hover:text-red-700"
                      title="Discontinue product"
                    >
                      <Ban className="h-4 w-4" />
                    </button>
                  )}
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export function ProductTableSkeleton() {
  return (
    <div className="overflow-x-auto rounded-lg border">
      <table className="w-full text-left text-sm">
        <thead className="border-b bg-gray-50 text-xs uppercase text-gray-500">
          <tr>
            <th className="px-6 py-3 w-16 text-center">#</th>
            <th className="px-6 py-3">Product Name</th>
            <th className="px-6 py-3">Category</th>
            <th className="px-6 py-3 text-right">Unit Price</th>
            <th className="px-6 py-3 text-center">Status</th>
            <th className="px-6 py-3 text-right">Actions</th>
          </tr>
        </thead>
        <tbody className="divide-y">
          {Array.from({ length: 5 }).map((_, i) => (
            <tr key={i}>
              <td className="px-6 py-4 text-center">
                <div className="mx-auto h-4 w-6 animate-pulse rounded bg-gray-200" />
              </td>
              <td className="px-6 py-4">
                <div className="h-4 w-40 animate-pulse rounded bg-gray-200" />
              </td>
              <td className="px-6 py-4">
                <div className="h-4 w-24 animate-pulse rounded bg-gray-200" />
              </td>
              <td className="px-6 py-4">
                <div className="ml-auto h-4 w-16 animate-pulse rounded bg-gray-200" />
              </td>
              <td className="px-6 py-4">
                <div className="mx-auto h-4 w-20 animate-pulse rounded bg-gray-200" />
              </td>
              <td className="px-6 py-4">
                <div className="ml-auto h-4 w-12 animate-pulse rounded bg-gray-200" />
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
