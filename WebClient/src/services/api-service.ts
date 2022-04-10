import { Specialty } from './../models/doctor';
import { Doctor } from "../models/doctor";

const API_URL = 'https://localhost:7101/';

export class ApiService {
	static async getDoctors(): Promise<Doctor[]> {
		const url = API_URL + 'Doctors?page=0&size=10';
		const response = await fetch(url);
		const data = await response.json();

		return data;

	}

	static async getSpecialties(){
		const url=API_URL+'Doctors/specialties';
		const response = await fetch(url);
		const data = await response.json();

		return data as Specialty[];
	}


}