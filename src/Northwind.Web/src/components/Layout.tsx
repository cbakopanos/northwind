import { NavLink, Outlet } from "react-router-dom";
import {
  LayoutDashboard,
  Package,
  Users,
  Truck,
  BarChart3,
  ShoppingCart,
  Building2,
  Factory,
} from "lucide-react";
import { cn } from "@/lib/utils";

const navItems = [
  { to: "/", label: "Home", icon: LayoutDashboard },
  { to: "/catalog", label: "Catalog", icon: Package },
  { to: "/crm", label: "CRM", icon: Users },
  { to: "/fulfillment", label: "Fulfillment", icon: Truck },
  { to: "/reporting", label: "Reporting", icon: BarChart3 },
  { to: "/sales-ordering", label: "Sales Ordering", icon: ShoppingCart },
  { to: "/sales-org", label: "Sales Org", icon: Building2 },
  { to: "/supplier", label: "Supplier", icon: Factory },
];

export function Layout() {
  return (
    <div className="flex h-screen">
      <aside className="flex w-56 flex-col border-r bg-gray-50">
        <div className="border-b px-4 py-4">
          <h1 className="text-lg font-bold tracking-tight">Northwind</h1>
        </div>
        <nav className="flex-1 space-y-1 p-2">
          {navItems.map(({ to, label, icon: Icon }) => (
            <NavLink
              key={to}
              to={to}
              end={to === "/"}
              className={({ isActive }) =>
                cn(
                  "flex items-center gap-2 rounded-md px-3 py-2 text-sm font-medium transition-colors",
                  isActive
                    ? "bg-gray-200 text-gray-900"
                    : "text-gray-600 hover:bg-gray-100 hover:text-gray-900"
                )
              }
            >
              <Icon className="h-4 w-4" />
              {label}
            </NavLink>
          ))}
        </nav>
      </aside>
      <main className="flex-1 overflow-auto p-6">
        <Outlet />
      </main>
    </div>
  );
}
