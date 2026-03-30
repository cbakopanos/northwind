import { Pencil } from "lucide-react";
import type { CustomerSummary } from "./types";

interface CrmTableProps {
  customers: CustomerSummary[];
  onEdit: (customerId: string) => void;
}

export function CrmTable({ customers, onEdit }: CrmTableProps) {
  return (
    <div className="overflow-x-auto rounded-lg border">
      <table className="w-full text-left text-sm">
        <thead className="border-b bg-gray-50 text-xs uppercase text-gray-500">
          <tr>
            <th className="px-6 py-3 w-24">ID</th>
            <th className="px-6 py-3">Company Name</th>
            <th className="px-6 py-3">Contact Name</th>
            <th className="px-6 py-3">Contact Title</th>
            <th className="px-6 py-3 text-right">Actions</th>
          </tr>
        </thead>
        <tbody className="divide-y">
          {customers.map((c) => (
            <tr key={c.customerId} className="hover:bg-gray-50">
              <td className="px-6 py-4">
                <span className="inline-flex h-6 items-center rounded-md bg-gray-100 px-2 text-sm font-mono text-gray-500">
                  {c.customerId}
                </span>
              </td>
              <td className="px-6 py-4 font-medium text-gray-900">{c.companyName}</td>
              <td className="px-6 py-4 text-gray-600">{c.contact.contactName ?? "—"}</td>
              <td className="px-6 py-4 text-gray-600">{c.contact.contactTitle ?? "—"}</td>
              <td className="px-6 py-4 text-right">
                <button
                  onClick={() => onEdit(c.customerId)}
                  className="rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-700"
                  title="Edit customer"
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

export function CrmTableSkeleton() {
  return (
    <div className="overflow-x-auto rounded-lg border">
      <table className="w-full text-left text-sm">
        <thead className="border-b bg-gray-50 text-xs uppercase text-gray-500">
          <tr>
            <th className="px-6 py-3 w-24">ID</th>
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
                <div className="h-4 w-12 animate-pulse rounded bg-gray-200" />
              </td>
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
