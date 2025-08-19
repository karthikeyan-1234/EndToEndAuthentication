import { Employee } from "../models/employee";

export class EmployeeService {
    private employees: Employee[] = [];

    addEmployee(employee: Employee) {
        this.employees.push(employee);
    }

    getEmployees(): Employee[] {
        return this.employees;
    }

    getEmployeeById(id: number): Employee | undefined {
        return this.employees.find(emp => emp.id === id);
    }

    updateEmployee(id: number, updatedEmployee: Employee): boolean {
        const index = this.employees.findIndex(emp => emp.id === id);
        if (index !== -1) {
            this.employees[index] = updatedEmployee;
            return true;
        }
        return false;
    }

    deleteEmployee(id: number): boolean {
        const index = this.employees.findIndex(emp => emp.id === id);
        if (index !== -1) {
            this.employees.splice(index, 1);
            return true;
        }
        return false;
    }
}
