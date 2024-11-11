@extends('layouts.app')
@section('title','メールマスタ')
@section('h1','メールマスタ')
@section('body')
    <table class="table table-bordered">
        @if(!empty($mails))
            <tr>
                <th>id</th>
                <th>アイテム名</th>
                <th>本文</th>
            </tr>
            @foreach($mails as $mail)
                <tr>
                    <td>{{$mail['id']}}</td>
                    <td>{{$mail->name}}</td>
                    <td>{{$mail['text_message']}}</td>
                </tr>
            @endforeach
        @endif
    </table>
@endsection


