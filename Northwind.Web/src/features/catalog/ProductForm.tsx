import { type FormEvent, useEffect, useRef, useState } from "react";
import { X, Loader2 } from "lucide-react";
import type { CategorySummary, ProductDetails, ProductRequest } from "./types";
import { useCategories } from "./useCategoryApi";

interface ProductFormProps {
  product: ProductDetails | null;
  isLoading: boolean;
  isSaving: boolean;
  error: string | null;
  onSave: (request: ProductRequest) => void;
  onClose: () => void;
}

const empty: ProductRequest = {
  productName: "",
  supplierId: null,
  categoryId: null,
  quantityPerUnit: null,
  unitPrice: 0,
  inventory: { unitsInStock: 0, unitsOnOrder: 0 },
  reorderLevel: 0,
};

function toRequest(p: ProductDetails): ProductRequest {
  return {
    productName: p.productName,
    supplierId: p.supplierId,
    categoryId: p.categoryId,
    quantityPerUnit: p.quantityPerUnit,
    unitPrice: p.unitPrice,
    inventory: p.inventory,
    reorderLevel: p.reorderLevel,
  };
}

export function ProductForm({
  product,
  isLoading,
  isSaving,
  error,
  onSave,
  onClose,
}: ProductFormProps) {
  const [form, setForm] = useState<ProductRequest>(empty);
  const { data: categoriesData } = useCategories();
  const categories = categoriesData ?? [];

  useEffect(() => {
    setForm(product ? toRequest(product) : empty);
  }, [product]);

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault();
    onSave(form);
  };

  const isNew = product === null;

  return (
    <div className="fixed inset-0 z-50 flex justify-end">
      <div className="fixed inset-0 bg-black/30" onClick={onClose} />

      <div className="relative z-10 flex w-full max-w-md flex-col bg-white shadow-xl">
        <div className="flex items-center justify-between border-b px-6 py-4">
          <h2 className="text-lg font-semibold">
            {isNew ? "Add Product" : "Edit Product"}
          </h2>
          <button
            onClick={onClose}
            className="rounded p-1 text-gray-400 hover:bg-gray-100 hover:text-gray-700"
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        {isLoading ? (
          <div className="flex flex-1 items-center justify-center">
            <Loader2 className="h-6 w-6 animate-spin text-gray-400" />
          </div>
        ) : (
          <form
            onSubmit={handleSubmit}
            className="flex flex-1 flex-col overflow-y-auto"
          >
            <div className="flex-1 space-y-4 px-6 py-4">
              {error && (
                <p className="rounded bg-red-50 px-3 py-2 text-sm text-red-600">
                  {error}
                </p>
              )}

              {!isNew && product && (
                <div className="flex items-center gap-2">
                  <span className="text-sm font-medium text-gray-700">ID</span>
                  <span className="inline-flex h-7 items-center rounded-md bg-gray-100 px-2.5 text-sm font-mono text-gray-500">
                    {product.productId}
                  </span>
                  {product.isDiscontinued && (
                    <span className="inline-flex items-center rounded-full bg-red-50 px-2.5 py-0.5 text-xs font-medium text-red-700">
                      Discontinued
                    </span>
                  )}
                </div>
              )}

              <Field
                label="Product Name"
                value={form.productName}
                onChange={(v) => setForm((f) => ({ ...f, productName: v }))}
                required
              />

              <div className="grid grid-cols-2 gap-3">
                <NumberField
                  label="Supplier ID"
                  value={form.supplierId}
                  onChange={(v) => setForm((f) => ({ ...f, supplierId: v }))}
                />
              </div>

              <CategoryCombobox
                key={product?.productId ?? "new"}
                categoryId={form.categoryId}
                initialName={product?.categoryName ?? null}
                categories={categories}
                onChange={(id) => setForm((f) => ({ ...f, categoryId: id }))}
              />

              <Field
                label="Quantity Per Unit"
                value={form.quantityPerUnit ?? ""}
                onChange={(v) =>
                  setForm((f) => ({ ...f, quantityPerUnit: v || null }))
                }
              />

              <NumberField
                label="Unit Price"
                value={form.unitPrice}
                onChange={(v) =>
                  setForm((f) => ({ ...f, unitPrice: v ?? 0 }))
                }
                step="0.01"
                min="0"
                required
              />

              <fieldset className="space-y-3">
                <legend className="text-xs font-medium uppercase text-gray-500">
                  Inventory
                </legend>
                <div className="grid grid-cols-2 gap-3">
                  <NumberField
                    label="Units In Stock"
                    value={form.inventory?.unitsInStock ?? 0}
                    onChange={(v) =>
                      setForm((f) => ({
                        ...f,
                        inventory: {
                          unitsInStock: v ?? 0,
                          unitsOnOrder: f.inventory?.unitsOnOrder ?? 0,
                        },
                      }))
                    }
                    min="0"
                  />
                  <NumberField
                    label="Units On Order"
                    value={form.inventory?.unitsOnOrder ?? 0}
                    onChange={(v) =>
                      setForm((f) => ({
                        ...f,
                        inventory: {
                          unitsInStock: f.inventory?.unitsInStock ?? 0,
                          unitsOnOrder: v ?? 0,
                        },
                      }))
                    }
                    min="0"
                  />
                </div>
              </fieldset>

              <NumberField
                label="Reorder Level"
                value={form.reorderLevel}
                onChange={(v) =>
                  setForm((f) => ({ ...f, reorderLevel: v ?? 0 }))
                }
                min="0"
              />
            </div>

            <div className="flex items-center justify-end gap-3 border-t px-6 py-4">
              <button
                type="button"
                onClick={onClose}
                className="rounded-md border px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
              >
                Cancel
              </button>
              <button
                type="submit"
                disabled={isSaving || !form.productName.trim()}
                className="inline-flex items-center gap-2 rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
              >
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

function CategoryCombobox({
  categoryId,
  initialName,
  categories,
  onChange,
}: {
  categoryId: number | null;
  initialName: string | null;
  categories: CategorySummary[];
  onChange: (id: number | null) => void;
}) {
  const [text, setText] = useState(initialName ?? "");
  const [open, setOpen] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    setText(initialName ?? "");
  }, [initialName]);

  const filtered = text.trim()
    ? categories.filter((c) =>
        c.categoryName.toLowerCase().includes(text.toLowerCase()),
      )
    : categories;

  const handleSelect = (c: CategorySummary) => {
    setText(c.categoryName);
    onChange(c.categoryId);
    setOpen(false);
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setText(e.target.value);
    onChange(null);
    setOpen(true);
  };

  const handleBlur = () => {
    // Delay so onMouseDown on an option fires before the list closes
    setTimeout(() => setOpen(false), 150);
  };

  return (
    <div ref={containerRef} className="relative">
      <label className="block">
        <span className="mb-1 block text-sm font-medium text-gray-700">
          Category
        </span>
        <input
          type="text"
          value={text}
          onChange={handleChange}
          onFocus={() => setOpen(true)}
          onBlur={handleBlur}
          autoComplete="off"
          placeholder="Search category…"
          className="w-full rounded-md border px-3 py-2 text-sm shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 focus:outline-none"
        />
      </label>
      {open && filtered.length > 0 && (
        <ul className="absolute z-20 mt-1 max-h-48 w-full overflow-y-auto rounded-md border bg-white text-sm shadow-lg">
          {filtered.map((c) => (
            <li
              key={c.categoryId}
              onMouseDown={() => handleSelect(c)}
              className={`cursor-pointer px-3 py-2 hover:bg-blue-50 ${
                c.categoryId === categoryId ? "bg-blue-50 font-medium" : ""
              }`}
            >
              {c.categoryName}
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

function Field({
  label,
  value,
  onChange,
  required,
}: {
  label: string;
  value: string;
  onChange: (value: string) => void;
  required?: boolean;
}) {
  return (
    <label className="block">
      <span className="mb-1 block text-sm font-medium text-gray-700">
        {label}
        {required && <span className="text-red-500"> *</span>}
      </span>
      <input
        type="text"
        value={value}
        onChange={(e) => onChange(e.target.value)}
        required={required}
        className="w-full rounded-md border px-3 py-2 text-sm shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 focus:outline-none"
      />
    </label>
  );
}

function NumberField({
  label,
  value,
  onChange,
  required,
  step,
  min,
}: {
  label: string;
  value: number | null;
  onChange: (value: number | null) => void;
  required?: boolean;
  step?: string;
  min?: string;
}) {
  return (
    <label className="block">
      <span className="mb-1 block text-sm font-medium text-gray-700">
        {label}
        {required && <span className="text-red-500"> *</span>}
      </span>
      <input
        type="number"
        value={value ?? ""}
        onChange={(e) =>
          onChange(e.target.value === "" ? null : Number(e.target.value))
        }
        required={required}
        step={step}
        min={min}
        className="w-full rounded-md border px-3 py-2 text-sm shadow-sm focus:border-blue-500 focus:ring-1 focus:ring-blue-500 focus:outline-none"
      />
    </label>
  );
}
