import { type FormEvent, useEffect, useState } from "react";
import { X, Loader2 } from "lucide-react";
import type { CustomerDetails, CustomerRequest } from "./types";

interface CrmFormProps {
  customer: CustomerDetails | null;
  isLoading: boolean;
  isSaving: boolean;
  error: string | null;
  onSave: (request: CustomerRequest) => void;
  onClose: () => void;
}

const empty: CustomerRequest = {
  companyName: "",
  contact: { contactName: null, contactTitle: null },
  address: { addressLine: null, city: null, region: null, postalCode: null, country: null },
  communication: { phone: null, fax: null, homepageUrl: null },
};

function toRequest(c: CustomerDetails): CustomerRequest {
  return {
    companyName: c.companyName,
    contact: c.contact,
    address: c.address,
    communication: c.communication,
  };
}

export function CrmForm({ customer, isLoading, isSaving, error, onSave, onClose }: CrmFormProps) {
  const [form, setForm] = useState<CustomerRequest>(empty);

  useEffect(() => {
    setForm(customer ? toRequest(customer) : empty);
  }, [customer]);

  const set = (field: string, value: string) => {
    const val = value || null;
    setForm((prev) => {
      if (field === "companyName") return { ...prev, companyName: value };

      if (field === "contactName" || field === "contactTitle")
        return { ...prev, contact: { ...prev.contact!, [field]: val } };

      if (field === "addressLine" || field === "city" || field === "region" || field === "postalCode" || field === "country")
        return { ...prev, address: { ...prev.address!, [field]: val } };

      if (field === "phone" || field === "fax" || field === "homepageUrl")
        return { ...prev, communication: { ...prev.communication!, [field]: val } };

      return prev;
    });
  };

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    onSave(form);
  };

  const isNew = customer === null;

  return (
    <div className="fixed inset-0 z-50 flex justify-end">
      <div className="fixed inset-0 bg-black/30" onClick={onClose} />

      <div className="relative z-10 flex w-full max-w-md flex-col bg-white shadow-xl">
        <div className="flex items-center justify-between border-b px-6 py-4">
          <h2 className="text-lg font-semibold">{isNew ? "Add Customer" : "Edit Customer"}</h2>
          <button onClick={onClose} className="rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-700">
            <X className="h-5 w-5" />
          </button>
        </div>

        {isLoading ? (
          <div className="flex flex-1 items-center justify-center">
            <Loader2 className="h-6 w-6 animate-spin text-gray-400" />
          </div>
        ) : (
          <form onSubmit={handleSubmit} className="flex flex-1 flex-col overflow-y-auto">
            <div className="flex-1 space-y-4 px-6 py-4">
              {error && <p className="rounded bg-red-50 px-3 py-2 text-sm text-red-600">{error}</p>}

              {!isNew && customer && (
                <div className="flex items-center gap-2">
                  <span className="text-sm font-medium text-gray-700">ID</span>
                  <span className="inline-flex h-7 items-center rounded-md bg-gray-100 px-2.5 text-sm font-mono text-gray-500">
                    {customer.customerId}
                  </span>
                </div>
              )}

              <Field label="Company Name" value={form.companyName} onChange={(v) => set("companyName", v)} required />

              <fieldset className="space-y-3">
                <legend className="text-xs font-medium uppercase text-gray-500">Contact</legend>
                <Field label="Contact Name" value={form.contact?.contactName ?? ""} onChange={(v) => set("contactName", v)} />
                <Field label="Contact Title" value={form.contact?.contactTitle ?? ""} onChange={(v) => set("contactTitle", v)} />
              </fieldset>

              <fieldset className="space-y-3">
                <legend className="text-xs font-medium uppercase text-gray-500">Address</legend>
                <Field label="Address Line" value={form.address?.addressLine ?? ""} onChange={(v) => set("addressLine", v)} />
                <div className="grid grid-cols-2 gap-3">
                  <Field label="City" value={form.address?.city ?? ""} onChange={(v) => set("city", v)} />
                  <Field label="Region" value={form.address?.region ?? ""} onChange={(v) => set("region", v)} />
                </div>
                <div className="grid grid-cols-2 gap-3">
                  <Field label="Postal Code" value={form.address?.postalCode ?? ""} onChange={(v) => set("postalCode", v)} />
                  <Field label="Country" value={form.address?.country ?? ""} onChange={(v) => set("country", v)} />
                </div>
              </fieldset>

              <fieldset className="space-y-3">
                <legend className="text-xs font-medium uppercase text-gray-500">Communication</legend>
                <Field label="Phone" value={form.communication?.phone ?? ""} onChange={(v) => set("phone", v)} />
                <Field label="Fax" value={form.communication?.fax ?? ""} onChange={(v) => set("fax", v)} />
                <Field label="Home Page" value={form.communication?.homepageUrl ?? ""} onChange={(v) => set("homepageUrl", v)} />
              </fieldset>
            </div>

            <div className="flex items-center justify-end gap-3 border-t px-6 py-4">
              <button type="button" onClick={onClose} className="rounded-md border px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50">Cancel</button>
              <button type="submit" disabled={isSaving || !form.companyName.trim()} className="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50">
                {isSaving && <Loader2 className="h-4 w-4 animate-spin" />}
                Save
              </button>
            </div>
          </form>
        )}
      </div>
    </div>
  );
}

function Field({ label, value, onChange, required }: { label: string; value: string; onChange: (value: string) => void; required?: boolean; }) {
  return (
    <label className="block">
      <span className="mb-1 block text-sm font-medium text-gray-700">{label}{required && <span className="text-red-500"> *</span>}</span>
      <input type="text" value={value} onChange={(e) => onChange(e.target.value)} required={required} className="w-full rounded-md border px-3 py-2 text-sm shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 focus:outline-none" />
    </label>
  );
}
