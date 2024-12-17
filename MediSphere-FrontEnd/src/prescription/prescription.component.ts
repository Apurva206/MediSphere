import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule, formatNumber } from '@angular/common';
import { jwtDecode } from 'jwt-decode';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPenToSquare } from '@fortawesome/free-solid-svg-icons';
import { faTrash, faFile } from '@fortawesome/free-solid-svg-icons';
@Component({
  selector: 'app-prescription',
  imports: [FormsModule,CommonModule, FontAwesomeModule],
  templateUrl: './prescription.component.html',
  styleUrl: './prescription.component.css'
})
export class PrescriptionComponent {
  faFile=faFile;
   faPenToSquare=faPenToSquare;
      faTrash=faTrash;
  PrescObj:any = {
    "prescriptionId": 0,
    "recordId": 0,
    "medicineName": "",
    "dosage": "",
    "frequency": "",
    "foodInstructions": ""
    }
  
    https = inject(HttpClient);
    onSubmit(){
      const token = localStorage.getItem('authToken');  // Retrieve the token
      const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
      this.https.post("https://localhost:7159/api/Prescriptions",this.PrescObj, { headers }).subscribe((res:any)=>{
        console.log('Prescription added successfully:', res);
        this.getAllUser(); // Refresh the list of appointments
        this.isAdding = false; 
        this.resetForm(); // Reset the form fields
        alert('Prescription added successfully!');
      })
    }
    ngOnInit(): void {
            const token = localStorage.getItem('authToken');
            if (token) {
              const decoded: any = jwtDecode(token);
              this.Username = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || '';
              this.role = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || '';
              this.userId = this.role === "Patient" ? decoded["PatientId"] : decoded["DoctorId"];
              this.prescriptionId = decoded["prescriptionId"] || null;  // Fetch the appointmentId if available in the token
              console.log('User Role:', this.role); // Check the role
              console.log('User ID:', this.userId);  // Check user ID
              console.log('Prescription ID:', this.prescriptionId);  // Check appointment ID
            }
          }
  
    onEdit(data : any){
      this.PrescObj=data;
    }
  
    onUpdate(){
      const prescriptionId = this.PrescObj.prescriptionId;
      const token = localStorage.getItem('authToken');  // Retrieve the token
      const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
      this.https.put(`https://localhost:7159/api/Prescriptions/${prescriptionId}`,this.PrescObj, { headers }).subscribe((res:any)=>{
        console.log('Prescription added successfully:', res);
        this.getAllUser(); // Refresh the list of appointments
        this.isAdding = false; 
        this.resetForm(); // Reset the form fields
        alert('Prescription updated successfully!');
      },
      (error: any) => {
        if (error.status === 404) {
            alert("You cannot update this Prescription.");
        }
    }
    )
    }
    userList:any [] = [];
    isAdding: boolean = false;
    role: string = '';
    Username:string='';
    prescriptionId: number = 0; 
  userId: number = 0;
    toggleForm() {
      this.isAdding = !this.isAdding; // Toggle the form
    }
    constructor(private http: HttpClient){
  
    }
    getAllUser(){
      const token = localStorage.getItem('authToken');  // Retrieve the token
      const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
      this.http.get("https://localhost:7159/api/Prescriptions",{ headers }).subscribe((result:any)=>{
        this.userList = result;
      })
    }
    getAppointmentById(prescriptionId: number) {
      const token = localStorage.getItem('authToken');
      const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
      this.https.get(`https://localhost:7159/api/Prescriptions/${prescriptionId}`, { headers }).subscribe((result: any) => {

        this.userList = [result];
        console.log('User appointments:', result);
        alert("Prescription Found!");
      },
      (error: any) => {
        if (error.status === 404) {
            alert("You can only access your own Prescription.");
        }
    });
    }
    onDelete(prescriptionId:number){
      const token = localStorage.getItem('authToken');  // Retrieve the token
      const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
      this.https.delete(`https://localhost:7159/api/Prescriptions/${prescriptionId}`, { headers }).subscribe((res:any)=>{
        console.log('Prescription deleted ', res);
        this.getAllUser(); // Refresh the list of appointments
        this.isAdding = false; 
        this.resetForm(); // Reset the form fields
        alert('Prescription deleted!');
      },
      (error: any) => {
        if (error.status === 401) {
            alert("You can only delete your own Prescription.");
        }
    })
    }
    addAppointment() {
      this.isAdding = true;
      this.resetForm();
    }
    resetForm() {
      this.PrescObj = {
    "prescriptionId": 0,
    "recordId": 0,
    "medicineName": "",
    "dosage": "",
    "frequency": "",
    "foodInstructions": ""
      };
    }
}
