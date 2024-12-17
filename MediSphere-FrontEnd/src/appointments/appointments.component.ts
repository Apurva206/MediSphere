import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { jwtDecode } from 'jwt-decode';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faCalendarCheck, faPenToSquare, faTrash } from '@fortawesome/free-solid-svg-icons';
@Component({
  selector: 'app-appointments',
  imports: [CommonModule,FormsModule, FontAwesomeModule],
  templateUrl: './appointments.component.html',
  styleUrl: './appointments.component.css'
})
export class AppointmentsComponent {
  faCalendarCheck=faCalendarCheck;
  faPenToSquare =faPenToSquare ;
  faTrash=faTrash;
  AppointmentObj:any = {
  "appointmentId": 0,
  "patientId": 0,
  "doctorId": 0,
  "appointmentDate": "",
  "status": "",
  "symptoms": "",
  "consultationNotes": ""
  }

  https = inject(HttpClient);
  onSubmit(){
    const token = localStorage.getItem('authToken');  // Retrieve the token
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    this.https.post("https://localhost:7159/api/Appointments",this.AppointmentObj, { headers }).subscribe((res:any)=>{
      console.log('Appointment added successfully:', res);
      this.getAllUser(); // Refresh the list of appointments
      this.isAdding = false; 
      this.resetForm(); // Reset the form fields
      alert('Appointment added successfully!');
    })
  }
  ngOnInit(): void {
    const token = localStorage.getItem('authToken');
    if (token) {
      const decoded: any = jwtDecode(token);
      this.Username = decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || '';
      this.role = decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || '';
      this.userId = this.role === "Patient" ? decoded["PatientId"] : decoded["DoctorId"];
      this.appointmentId = decoded["AppointmentId"] || null;  // Fetch the appointmentId if available in the token
      console.log('User Role:', this.role); // Check the role
      console.log('User ID:', this.userId);  // Check user ID
      console.log('Appointment ID:', this.appointmentId);  // Check appointment ID
    }
  }
  
  onEdit(data : any){
    this.AppointmentObj=data;
  }

  onUpdate(){
    const appointmentId = this.AppointmentObj.appointmentId;
    const token = localStorage.getItem('authToken');  // Retrieve the token
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    this.https.put(`https://localhost:7159/api/Appointments/${appointmentId}`,this.AppointmentObj, { headers }).subscribe((res:any)=>{
      console.log('Appointment added successfully:', res);
      this.getAllUser(); // Refresh the list of appointments
      this.isAdding = false; 
      this.resetForm(); // Reset the form fields
      alert('Appointment updated successfully!');
    },
    (error: any) => {
      if (error.status === 403) {
          alert("You cannot update this appointment. Doctors can only update their own appointments.");
      }
  }
  )
  }
  selectedStatus: string = ''; // Default filter value for dropdown
  filteredList: any[] = [];
  userList:any [] = [];
  isAdding: boolean = false;
  role: string = '';
  Username: string='';
  appointmentId: number = 0; 
  userId: number = 0;
  toggleForm() {
    this.isAdding = !this.isAdding; // Toggle the form
  }
  constructor(private http: HttpClient){

  }
  getAllUser(){
    const token = localStorage.getItem('authToken');  // Retrieve the token
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    this.http.get("https://localhost:7159/api/Appointments",{ headers }).subscribe((result:any)=>{
      this.userList = result;
      this.applyFilter();
    })
  }

  getAppointmentById(appointmentId: number) {
    const token = localStorage.getItem('authToken');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    this.https.get(`https://localhost:7159/api/Appointments/${appointmentId}`, { headers }).subscribe((result: any) => {
      this.userList = [result];
      console.log('User appointments:', result);
      alert("Appointment Found!");
    },
    (error: any) => {
      if (error.status === 404) {
          alert("You can only access your own Appointments.");
      }
  });
  }
  
  onDelete(appointmentId:number){
    const token = localStorage.getItem('authToken');  // Retrieve the token
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    this.https.delete(`https://localhost:7159/api/Appointments/${appointmentId}`, { headers }).subscribe((res:any)=>{
      console.log('Appointment deleted ', res);
      this.getAllUser(); // Refresh the list of appointments
      this.isAdding = false; 
      this.resetForm(); // Reset the form fields
      alert('Appointment deleted!');
    },
    (error: any) => {
      if (error.status === 401) {
          alert("You can only delete your own Appointments.");
      }
  })
  }
  applyFilter() {
    if (this.selectedStatus === '') {
      this.filteredList = [...this.userList]; // Show all appointments if no filter
    } else {
      this.filteredList = this.userList.filter(item => item.status === this.selectedStatus);
    }
  }
  addAppointment() {
    this.isAdding = true;
    this.resetForm();
  }
  resetForm() {
    this.AppointmentObj = {
      appointmentId: 0,
      patientId: 0,
      doctorId: 0,
      appointmentDate: '',
      status: '',
      symptoms: '',
      consultationNotes: ''
    };
  }
}
