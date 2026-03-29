import { Pencil } from "lucide-react";
import type { SupplierSummary } from "./types";

interface SupplierTableProps {
  suppliers: SupplierSummary[];
  onEdit: (supplierId: number) => void;
}

export function SupplierTable({ suppliers, onEdit }: SupplierTableProps) {
  return (
    <div className="overflow-x-auto rounded-lg border">
      <table className="w-full text-left text-sm">
        <thead className="border-b bg-gray-50 text-xs uppercase text-gray-500">
          <tr>
            <th className="px-6 py-3">Company Name</th>
            <th className="px-6 py-3">Contact Name</th>
            <th className="px-6 py-3">Contact Title</th>
            <th className="px-6 py-3 text-right">Actions</th>
          </tr>
        </thead>
        <tbody className="divide-y">
          {suppliers.map((s) => (
            <tr key={s.supplierId} className="hover:bg-gray-50">
              <td className="px-6 py-4 font-medium text-gray-900">
                {s.companyName}
              </td>
              <td className="px-6 py-4 text-gray-600">
                {s.contact.contactName ?? "—"}
              </td>
              <td className="px-6 py-4 text-gray-600">
                {s.contact.contactTitle ?? "—"}
              </td>
              <td className="px-6 py-4 text-right">
                <button
                  onClick={() => onEdit(s.supplierId)}
                  className="rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-700"
                  title="Edit supplier"
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

export function SupplierTableSkeleton() {
  return (
    <div className="overflow-x-auto rounded-lg border">
      <table className="w-full text-left text-sm">
        <thead className="border-b bg-gray-50 text-xs uppercase text-gray-500">
          <tr>
            <th className="px-6 py-3">Company Name</th>
            <th className="px-6 py-3">Contact Name</th>
            <th className="px-6 py-3">Contact Title</th>
            <th className="px-6 py-3 text-right">Actions</th>
          </tr>
        </thead>
        <tbody className="divide-y">
          {Array.from({ length: 5 }).map((_, i) => (
            <tr key={i}>
              <td className="px-6 py-4">
                <div className="h-4 w-40 animate-pulse rounded bg-gray-200" />
              </td>
              <td className="px-6 py-4">
                <div className="h-4 w-28 animate-pulse rounded bg-gray-200" />
              </td>
              <td className="px-6 py-4">
                <div className="h-4 w-32 animate-pulse rounded bg-gray-200" />
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
