import React from "react";
import { useForm } from "react-hook-form";

export default function Form() {
    const { register, handleSubmit, formState:{errors, isLoading}} = useForm();

    const login = (data) => {
        console.log(data);
    }
    return (
     <div>
        <form onSubmit={handleSubmit(login)}>
            <div>
            <input
            {...register("userName")}
            />
            {errors.userName && <p style={{color: 'red'}}>{errors.userName.message}</p>}
            </div>
            <input
            {...register("password")}
            />
            <button disabled={isLoading} type="submit"/>
            <button>login</button>
        </form>
    </div>
    );
}