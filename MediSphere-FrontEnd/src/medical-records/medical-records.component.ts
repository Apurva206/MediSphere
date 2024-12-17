import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { jwtDecode } from 'jwt-decode';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faNotesMedical, faPenToSquare, faTrash } from '@fortawesome/free-solid-svg-icons';
@Component({
  selector: 'app-medical-records',
  imports: [FormsModule,CommonModule, FontAwesomeModule],
  templateUrl: './medical-records.component.html',
  styleUrl: './medical-records.component.css'
})
export class MedicalRecordsComponent {
  faNotesMedical=faNotesMedical;
  faPenToSquare=faPenToSquare;
  faTrash=faTrash;
  RecordObj:any = {
    "recordId": 0,
    "patientId": 0,
    "doctorId": 0,
    "appointmentId": "",
    "symptoms": "",
    "consultationDates": "",
    "prescribedTests": ""
    }
  
    https = inject(HttpClient);
    onSubmit(){
      const token = localStorage.getItem('authToken');  // Retrieve the token
      const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
      this.https.post("https://localhost:7159/api/MedicalRecords",this.RecordObj, { headers }).subscribe((res:any)=>{
        console.log('Record added successfully:', res);
        this.getAllUser(); // Refresh the list of appointments
        this.isAdding = false; 
        this.resetForm(); // Reset the form fields
        alert('Record added successfully!');
      })
    }
    ngOnInit(): void {
        const token = localStorage.getItem('authToken');
        if (token) {
          const decoded: any = jwtDecode(token);
          this.Username = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || '';
          this.role = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || '';
          this.userId = this.role === "Patient" ? decoded["PatientId"] : decoded["DoctorId"];
          this.recordId = decoded["RecordId"] || null;  // Fetch the appointmentId if available in the token
          console.log('User Role:', this.role); // Check the role
          console.log('User ID:', this.userId);  // Check user ID
          console.log('Record ID:', this.recordId);  // Check appointment ID
        }
      }
  
    onEdit(data : any){
      this.RecordObj=data;
    }
  
    onUpdate(){
      const recordId = this.RecordObj.recordId;
      const token = localStorage.getItem('authToken');  // Retrieve the token
      const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
      this.https.put(`https://localhost:7159/api/MedicalRecords/${recordId}`,this.RecordObj, { headers }).subscribe((res:any)=>{
        console.log('Record added successfully:', res);
        this.getAllUser(); // Refresh the list of appointments
        this.isAdding = false; 
        this.resetForm(); // Reset the form fields
        alert('Record updated successfully!');
      },
      (error: any) => {
        if (error.status === 403) {
            alert("You cannot update this Record. Doctors can only update their own Patient records.");
        }
    }
    )
    }
    userList:any [] = [];
    isAdding: boolean = false;
    role: string = '';
    Username:string='';
  recordId: number = 0; 
  userId: number = 0;
    toggleForm() {
      this.isAdding = !this.isAdding; // Toggle the form
    }
    constructor(private http: HttpClient){
  
    }
    getAllUser(){
      const token = localStorage.getItem('authToken');  // Retrieve the token
      const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
      this.http.get("https://localhost:7159/api/MedicalRecords",{ headers }).subscribe((result:any)=>{
        this.userList = result;
      })
    }
    getAppointmentById(recordId: number) {
      const token = localStorage.getItem('authToken');
      const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
      this.https.get(`https://localhost:7159/api/MedicalRecords/${recordId}`, { headers }).subscribe((result: any) => {

        this.userList = [result];
        console.log('User appointments:', result);
        alert("Record Found!");
      },
      (error: any) => {
        if (error.status === 404) {
            alert("You can only access your own medical records.");
        }
    });
    }
    onDelete(recordId:number){
      const token = localStorage.getItem('authToken');  // Retrieve the token
      const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
      this.https.delete(`https://localhost:7159/api/MedicalRecords/${recordId}`, { headers }).subscribe((res:any)=>{
        console.log('Record deleted successfully:', res);
        this.getAllUser(); // Refresh the list of appointments
        this.isAdding = false; 
        this.resetForm(); // Reset the form fields
        alert('Record deletd successfully!');
      },
      (error: any) => {
        if (error.status === 401) {
            alert("You can only delete your own Medical Records.");
        }
    })
    }
    addRecord() {
      this.isAdding = true;
      this.resetForm();
    }
    resetForm() {
      this.RecordObj = {
    "recordId": 0,
    "patientId": 0,
    "doctorId": 0,
    "appointmentId": "",
    "symptoms": "",
    "consultationDates": "",
    "prescribedTests": ""
      };
    }
}
