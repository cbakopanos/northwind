import { NavLink, Outlet } from "react-router-dom";
import { cn } from "@/lib/utils";

const tabs = [
  { to: "products", label: "Products" },
  { to: "categories", label: "Categories" },
];

export function CatalogPage() {
  return (
    <div>
      <h1 className="mb-4 text-2xl font-bold">Catalog</h1>

      <nav className="mb-6 flex gap-1">
        {tabs.map((tab) => (
          <NavLink
            key={tab.to}
            to={tab.to}
            className={({ isActive }) =>
              cn(
                "rounded-md px-4 py-2 text-sm font-medium transition-colors",
                isActive
                  ? "bg-slate-900 text-white"
                  : "text-slate-500 hover:bg-slate-100 hover:text-slate-800",
              )
            }
          >
            {tab.label}
          </NavLink>
        ))}
      </nav>

      <Outlet />
    </div>
  );
}
