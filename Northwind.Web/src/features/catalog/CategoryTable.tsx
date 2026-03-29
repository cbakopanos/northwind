import { Pencil } from "lucide-react";
import type { CategorySummary } from "./types";

interface CategoryTableProps {
  categories: CategorySummary[];
  onEdit: (categoryId: number) => void;
}

export function CategoryTable({ categories, onEdit }: CategoryTableProps) {
  return (
    <div className="overflow-x-auto rounded-lg border">
      <table className="w-full text-left text-sm">
        <thead className="border-b bg-gray-50 text-xs uppercase text-gray-500">
          <tr>
            <th className="px-6 py-3 w-16 text-center">#</th>
            <th className="px-6 py-3 w-24 text-center">Picture</th>
            <th className="px-6 py-3">Category Name</th>
            <th className="px-6 py-3 text-right">Actions</th>
          </tr>
        </thead>
        <tbody className="divide-y">
          {categories.map((c) => (
            <tr key={c.categoryId} className="hover:bg-gray-50">
              <td className="px-6 py-4 text-center">
                <span className="inline-flex h-6 min-w-6 items-center justify-center rounded-full bg-gray-100 px-2 text-xs font-medium text-gray-500">
                  {c.categoryId}
                </span>
              </td>
              <td className="px-6 py-4 text-center">
                {c.hasPicture ? (
                  <img
                    src={`/api/catalog/categories/${c.categoryId}/picture`}
                    alt={c.categoryName}
                    className="mx-auto h-10 w-10 rounded object-cover"
                  />
                ) : (
                  <span className="text-xs text-gray-400">—</span>
                )}
              </td>
              <td className="px-6 py-4 font-medium text-gray-900">
                {c.categoryName}
              </td>
              <td className="px-6 py-4 text-right">
                <button
                  onClick={() => onEdit(c.categoryId)}
                  className="rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-700"
                  title="Edit category"
                >
                  <Pencil className="h-4 w-4" />
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export function CategoryTableSkeleton() {
  return (
    <div className="overflow-x-auto rounded-lg border">
      <table className="w-full text-left text-sm">
        <thead className="border-b bg-gray-50 text-xs uppercase text-gray-500">
          <tr>
            <th className="px-6 py-3 w-16 text-center">#</th>
            <th className="px-6 py-3 w-24 text-center">Picture</th>
            <th className="px-6 py-3">Category Name</th>
            <th className="px-6 py-3 text-right">Actions</th>
          </tr>
        </thead>
        <tbody className="divide-y">
          {Array.from({ length: 5 }).map((_, i) => (
            <tr key={i}>
              <td className="px-6 py-4 text-center">
                <div className="mx-auto h-4 w-6 animate-pulse rounded bg-gray-200" />
              </td>
              <td className="px-6 py-4 text-center">
                <div className="mx-auto h-10 w-10 animate-pulse rounded bg-gray-200" />
              </td>
              <td className="px-6 py-4">
                <div className="h-4 w-40 animate-pulse rounded bg-gray-200" />
              </td>
              <td className="px-6 py-4">
                <div className="ml-auto h-4 w-6 animate-pulse rounded bg-gray-200" />
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
